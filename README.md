# Content Cloud Static Websites

This library allows you to serve entire sections of static-file based content from Content Cloud which are _managed as content_, not code.

The sites can either be the entire domain, or just a branch of URLs. It effectively turns a single page into a web server which will respond to requests with the contents of managed files. Those files can be updated directly from the Edit Mode UI and do not require a code deploy.

The existence of a static site does not affect any other functionality in Content Cloud. If it's confined to a branch of the page tree, it will be used only for URLs under its point in the tree. Additionally, it does not affect the operation of any of the headless content APIs.

[Why would someone want to do this?](docs/why.md) (I promise there are some good reasons.)

## How It Works

Static Sites are accomplished via a "partial router," which is a Content Cloud feature (specifically, it's an implementation of the `IPartialRouter<T,T>` interface).

In Content Cloud, URLs are resolved segment-by-segment, from left to right. The leftmost segment is matched to a page, then the next segment is matched to a child page of the first one, and so on, until all segments have been resolved and the desired page is produced.

A partial router can interrupt this process when it finds a specified page type. In general terms, when the resolution process produces a  page of this type for any segment, the partial router will activate, intercept the rest of the URL (all the segments to the "right" of the current page), and change what page is produced.

For example, partial routers are often used for pagination, so you can enable URLs like `/news/page/2`, etc. The `page/2` part never exists as an actual page, but a partial router would activate on whatever page it finds at `/news/`, abandon further resolution, and pass `page/2` to the controller.

The Static Site system works by introducing a page type called `StaticSiteRoot` and a partial router which is bound to it. When this page type is produced in the URL resolution process (at any segment), further resolution is abandoned, so the same `StaticSiteRoot` page will be returned for *all* URLs "downstream" of its location. The remaining path will then be passed to the `StaticSiteRoot` page's controller to produce output.

This path is then used to locate a static file in a zip archive which is attached to the the `StaticSiteRoot` page as a content asset. Thus, to change a static site, you simply need to upload a new zip file asset to its root.

(Note: the system is mainly comprised of injected services. Any of these services can be swapped out, which can radically alter how it functions. This document explains how it works with the default, built-in service implementations.)

## Getting Started

### The Static Site

1. Create a static HTML page called `index.html`. Put some content in it.
2. Create a directory called `images` next to your HTML file. Put an image in it.
3. Add the image to your HTML file via an `IMG` tag with a relative URL (so: `images/[image-name.jpg]`).
2. Zip these files (note: zip _the file and folder themselves_, not the directory they're contained in)

(Clearly, this is very simple. Your static site can be comprised of whatever you want, this example is just the simplest possible static site we can create.)

### Content Cloud Usage

1. Compile the `src` directory into your project
2. In Edit Mode, add and publish a `StaticSiteRoot` page somewhere in the tree
3. Add your zip file as an asset _for that page_ (at the default config, the file should publish automatically; if you changed this setting, you'll need to publish it)

Request the `StaticSiteRoot` in your browser. You should see the contents of `index.html`. It should load the referenced image from the subdirectory.

If you add `images/[image-name.jpg]` onto the URL of `StaticSiteRoot` page, you should see the image directly.

## Commands

There are several "magic" URL paths that will perform actions or provide information when called on a `StaticSiteRoot` (note: the below are *double* underscores):

* **__contents:** Will display the contents of the zip archive
* **__context:** Will return a JSON payload of information about the static site:
  * **rootID:** The content object ID for the site
  * **baseUrl:** The base path to the site
  * **userName:** The username of the authenticated user. If unauthenticated, this will be `null`
* **__cache:** The paths which have been cached for a static site
* **__clear:** Clears the cache of a static site
* **__asset/[filename]:** Returns the content of a file stored outside the zip archive, in the local assets for the static site content object. This is intended to be used as the target of a `LINK`, `SCRIPT`, or `IMG` tag.

This is extensible. New commands can be registered in `StaticSiteCommands`.

Presently, none of these command paths are authenticated.

## Path Translation

To translate an inbound path to a path that can be retrieved, the default service (derived from `IStaticPathTranslator`) follows this logic:

* The inbound URL (in full) is trimmed from the start with the URL from the `StaticSiteRoot`
* If the remaining URL ends in a slash, then the default document is appended (by default, this is `index.html`, but it's a public property on `IStaticPathTranslator` if you want to change ut)
* The leading slash is trimmed

So, if the `StaticSiteRoot` is at `/foo/bar/` and you request `/foo/bar/baz/`, that will be translated into `baz/index.html` for retrieval by `IStaticSiteResourceProvider`.

If nothing is found, the resource provider will look for something at `404.html` (also a public property on `IStaticPathTranslator`, if you want to change it). If it finds something there, the contents will be retuned with a 404 status code. If nothing is found there, the controller will return a `NotFoundResult` which will be handled however you configured it.

## MIME Type Determination

After path translation, there should always be a file extension. I delegate MIME determination to the `FileExtensionContentTypeProvider` (this is handled in `IMimeTypeManager` if you want to customize it).

Based on the MIME type, I have some logic to determine if the resource contents are text or not (so I know whether to return a `ContentResult` or `FileContentResult`). The logic is:

* If the MIME starts with `text/` then it's text.
* If the MIME ends with `+xml` then it's text (there are a bunch of weird ones like this)
* `MimeTypeManager` has a default list of seven other text MIMEs (like, `application/javascript` etc; it's a public property, if you want to change it)
* If it doesn't resolve as text by this point, then it's not text

## Status

Wildly alpha and totally unsupported.

This is a hobby/side project of an Optimizely employee. It is _not_ part of the product, and it's _not_ a Labs project.

Honestly, I'm not even totally sure this is a good idea. It's basically an academic pursuit at the moment.
