# Why do this?

In developing this library (architecture? convention?), I asked some people for feedback, and the most common thing I heard was: "...why would someone want to do this?"

To be clear: _the need for something like this is an edge case_. I am emphatically _not_ advocating for all your content to be built statically.

But I've been fascinated with the idea of mixing CMS and static HTML for a long time:

* From 2005: [Middle Ground: Content Management using Static HTML](https://deanebarker.net/tech/blog/middle-ground-content-management-using-static-html/)
* From 2006: [Your CMS Isn’t Too Good for Static HTML](https://deanebarker.net/tech/blog/your-cms-isnt-too-good-for-static-html/)

I've also built a (little-known) library to inject static HTML and resources into blocks inside managed pages:

* [Episerver Markup](https://github.com/deanebarker/Episerver-Markup/) (note: I have not updated this to CMS12, but it wouldn't be hard)

So, my interest in this is long-standing. And in consulting with clients about CMS problems for 20-some-odd years, there are some situations where this would have been helpful:

## The "Head" For a Headless Solution

Content Cloud is a fine headless CMS at its core. It can be used to manage content with no presentation and deliver content to any channel, including a browser-based JavaScript app.  But, where do you host that app?  If the CMS is "headless," where is the head?

You can host a single static website inside the Content Cloud web app, because it will serve up static content and files normally from its root. But what if you want it to be on a separate DevOps flow than the Content Cloud instance itself? That gets harder because you're mixing front-end and back-end code, and the goal for many headless scenarios is distribution of work and DevOps.

The ability to serve static websites from a Content Cloud instance means you can serve up unlimited "heads," all talking to the same headless instance running in the background -- all the static sites have access to the same repo via the Content Delivery API on their own domain. These sites can be updated without redeploying the Content Cloud instance.

(In this use case the instance might have two websites configured -- one "normal" CMS site where content is edited, and one which serves the static head as the Start Page. Or you could do it with one site, and just configure an alternate editing domain.)

## Generated Static Sites

Static site generation is becoming popular (again). This is happening both as a solution to realtime processing issues, and because static sites can come from a single publishing entity and result in a set portable artifacts (example: generated API documentation).

But how do you host and serve this from your CMS domain?  Deploying static files to a CMS-managed site can be complex -- if they're stored as managed assets, they might have to be uploaded and managed individually. Additionally, most CMSs do not offer "URL fidelity" for asset URLs, so links that work when the site is generated will rarely survive being stored in a CMS repository.

(Truth: the originally goal here was to allow you to upload a zip file and then have it extract to managed assets. But I couldn't figure out the value prop there -- having them as managed asset when you would never edit them in the CMS just didnt make sense. The only thing it seemed to help with was exposing individual URLs to search.)

Additionally, having static sites proxied through a CMS allows you to bring some CMS services to bear on them -- like, authentication. Since this system requires all requests to funnel through a single content object, access control  -- or other settings -- on that object will extend to all the static content contained "within" it.

## Microsites and One-Off Sections

Often content is hand-built by front-end developers due to intricate or bespoke CSS and JavaScript requirements. This is often for temporary or limited content pages or sections -- a campaign landing page or microsite or special section, for example. This might be a single page which requires multiple linked assets to function (CSS, JS, images, etc.)

In these cases, the design work is heavy and is not likely to be repeated in exactly that form, so there's little gain from converting all the design concepts into page, block, and media types, which would just bottleneck on the C# developer. If the content was complex enough to require specialized front-end development, it's not likely to translate to managed content elements easily, or provide much value in that form, especially if the content has a limited life (for example: a highly designed and customized section for your industry conference which is happening in three months, after which all the content is thrown away).

In these cases, most of the desired post-publish optimization tools (experimentation, customer data capture, etc.) can be activated from the client. Any needs for actual managed content can be injected via the Content Delivery API, essenitally making it a small headless application.

[I'll keep updating this as I remember war stories from my years in implementation services...]


