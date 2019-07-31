# RemoteRazorPartials
Loading partial page content from a remote source within a razor page

The usage is fairly simple, just copy the files RazorExtensions and RemoteRazorPageViewsConfiguration and use the provided extension method, e.g.

```
@await  Html.RenderRemotePartialAsync("https://google.com")
```

Check the sample for further details and customizations regarding Polly retry policies.
