# Content Cloud Response Providers

This library allows you to provide responses from external sources for entire sections of the URL path to a Content Cloud instance -- even the entire path, if you like.

For example:

* A site or section in Content Cloud could be served from HTML, JS, and CSS files stored in the CMS itself as a zip file media asset
* A site or section could be served the same way, except from a directory of files on the server's file system
* A site or section could be served by making server-side HTTP calls to another website; effectively a proxy

(Note: these three options are provided out of the box.)

It's noted as "site or section," because this method works if used from the start page (so the entire site is served this way), or from a page further down the tree (so it only activates on paths "below" that page).

[Why would someone want to do this?](docs/why.md) (I promise there are some good reasons.)

It's quite easy to extend, so responses can theoretically formed from any external resource.

## Background: How Partial Routers Work

This functionality is accomplished via a "partial router," which is a feature of the Content Cloud API (specifically, it's an implementation of the `IPartialRouter<T,T>` interface).

In Content Cloud, URLs are resolved segment-by-segment, from left to right. The leftmost segment is matched to a page, then the next segment is matched to a child page of the first one, and so on, until all segments have been resolved and the desired page is produced.

A partial router can interrupt this process when it finds a specified page type. In general terms, when the resolution process produces a page of this type for any segment, the partial router will activate, intercept the rest of the URL (all the segments to the "right" of the current page), and change what page is produced.

For example, partial routers are often used for pagination, so you can enable URLs like `/news/page/2`, etc. The `page/2` part never exists as an actual page, but a partial router would activate on whatever page it finds at `/news/`, abandon further resolution, and pass `page/2` to the controller.

The resource provider system works by introducing a base page type called `BaseResponseProvider` and a partial router which is bound to it. When any content that extends from this type is produced in the URL resolution process (at any segment), further resolution is abandoned, so the same `BaseResponseProvider` page will be returned for *all* URLs "downstream" of its location. The remaining path will then be passed to the `ResponseProviderController` to produce output.

For example:

* This path can be used to locate a static file in a zip archive which is attached to the the `BaseResponseProvider` page as a content asset. Thus, to change a static site, you simply need to upload a new zip file asset to its root.
* This path can be used to locate a file on the file system
* This path can be used to make an HTTP proxy call to another website

(Note: these three options are provided out of the box.)

## Getting Started

Create a static website on your local file system. For now, keep all the paths (IMG, SCRIPT, LINK, etc.) relative (so they don't start with "/"), and the default document should be `index.html` (all this can be changed later).

For testing, this could be as simple as a single "index.html" file.

Zip all the files (_not_ the directory they're in; highlight all the files, and zip them directly).

### Content Cloud Usage

1. Compile the `src` directory into your project
2. In Edit Mode, add and publish a `Zip Archive Site Root` page somewhere in the tree
3. Add your zip file as an asset _for that page_ (at the default config, the file should publish automatically; if you changed this setting, you'll need to publish it)

Request the `Zip Archive Site Root` in your browser. You should see the contents of `index.html`. Any embedded links -- `A@HREF`, `LINK@HREF, `SCRIPT@SRC`, `IMG@SRC`, etc. -- should also work.

## Service Descriptions

[Service Descriptions](docs/services.md)

## Commands

There are several "magic" URL paths that will perform actions or provide information when called on a `BaseResponseProvider` (note: the below are *double* underscores):

* **__contents:** Will display the contents of external resource (the zip file, in our example above)

* **__context:** Will return a JSON payload of information about the resource provider's mounting point, and the current user

  * **rootID:** The content object ID for the resource provider
  
  * **baseUrl:** The base path to the resource provider
  
  * **userName:** The username of the authenticated user. If unauthenticated, this will be `null`

* **__cache:** The paths which have been cached for a resource provider

* **__clear:** Clears the cache of a resource provider

This is extensible. New commands can be registered in `ResponseProviderCommandManager`.

Presently, none of these command paths are authenticated.

## Path Translation

The inbound path is passed to an implementation of `IResponseProviderPathTranslator` to get a path that can be used on the external resource.

### ProxyResourceProvider

This uses `SimplePathTranslator` which does the following:

* The inbound URL (in full) is trimmed from the start with the URL from the `BaseResponseProvider`
* The leading slash is trimmed

So, if the `BaseResponseProvider` is at `/foo/bar/` and you request `/foo/bar/baz/`, that will be translated into `baz` for retrieval by `ISourceProvider`.

### ZipArchiveResourceProvider and FileSystemResourceProvider

These two resource providers use the `FileSystemPathTranslator`, which extends from `SimplePathTranslator`. It just adds another step:

* If the remaining URL ends in a slash, then the default document is appended (by default, this is `index.html`, but it's a public property on `FileSystemPathTranslator` if you want to change ut)

So, if the `BaseResponseProvider` is at `/foo/bar/` and you request `/foo/bar/baz/`, that will be translated into `baz/index.html` for retrieval by `ISourceProvider`.

## Resource Retrieval

The translated path (from above) is passed to an implementation of `ISourceProvider`. That service responds with a payload of bytes which represents the resource.

### ZipArchiveResourceProvider

This implementation retrieves the resource from a zip file stored in the repository. It uses the following logic to find the asset (it uses the first one that it finds):

1. If the `ArchiveFile` property is populated, it will use that reference (this is so multiple static sites can use the same zip file of assets)
2. If an asset attached to the `ResponseProviderRoot` is named `_source.zip`
3. The first `.zip` asset it finds attached to the content object

(In most cases, falling through to #3 is fine. You only need to do #1 if you want to use the same file in multiple places, and only need to use #2 if you'll have more than one zip file attached to the object.)

Once the zip file is located, the translated path is used to return the bytes of the resource.

There is no need to know or care what the type of resource is (HTML file, image, etc.). The response is formed solely from the bytes, and the `ContentType` is generated from the requested path (see below).

### FileSystemResourceProvider

This implementation appends the translated path onto a file system path stored on the `FileSystemResourceProvider` content object itself. It reads the resulting path from the server's file system.

### ProxyResourceProvider

This implementation appends the translated path path onto a URL stored on the `ProxyResourceProvider` content object itself. It makes an HTTP GET request and returns the resulting bytes.

## MIME/Content Type Determination

After path translation, there should always be a file extension. MIME determination is delegated to the `FileExtensionContentTypeProvider` (this is handled in `IMimeTypeManager` if you want to customize it).

Based on the MIME type, I have some logic to determine if the resource contents are text or not (so I know whether to return a `ContentResult` or `FileContentResult`). The logic is:

* If the MIME starts with `text/` then it's text.
* If the MIME ends with `+xml` then it's text (there are a bunch of weird ones like this)
* `MimeTypeManager` has a default list of seven other text MIMEs (like, `application/javascript` etc; it's a public property, if you want to change it)
* If it doesn't resolve as text by this point, then it's not text

## Resource Transformation

Static resources can be transformed after retrieval, before they are sent back in the response. Some examples:

1. A transformer could ensure there was a consistent `DOCTYPE` at the top of every HTML file
2. A transformer could resize images based on a querystring argument
3. A transformer could process CSS shorthand files, like Sass or Less

Transformation is _not_ done in "real time." The transforms are cached for future requests.

A transformer is a class that implements `ITransformer` and provides a `Transform` method. Instances of these classes need to be registered with the `IResponseProviderTransformerManager` service.

```csharp
var _responseProviderTransformerManager = ServiceLocator.Current.GetInstance<IResponseProviderTransformerManager>();
_responseProviderTransformerManager.Transformers.Add(new AddScriptTag("http://example.com/deane.js"));
_responseProviderTransformerManager.Transformers.Add(new RemoveRemoteScripts());
_responseProviderTransformerManager.Transformers.Add(new EnsureDocType());
```

_Every_ registered transformer executes for _every_ resource. You need to provide logic inside `Transform` to control when it _should_ alter the bytes. For example:

```csharp
public byte[] Transform(byte[] content, string path, BaseResponseProvider root, string mimeType);
{
  if (mimeType != "text/html") return content; // Not HTML; abandon
  
  // Do stuff to the byte[] in "content"
  
  return content
}
```

## Status

Wildly alpha and totally unsupported.

This is a hobby/side project of an Optimizely employee. It is _not_ part of the product, and it's _not_ a Labs project.

Honestly, I'm not even totally sure this is a good idea. It's basically an academic pursuit at the moment.
