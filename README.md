# CoreApi
ASP.NET Core Web API. Proxy service for Coinmarketcap API endpoints

This is written as a basic example of implementing proxy service for Coinmarketcap's pro API.

This is necessary to overcome the following limitation imposed by CMC:

"Making HTTP requests on the client side with Javascript is currently prohibited through CORS configuration. This is to protect your API Key which should not be visible to users of your application so your API Key is not stolen. Secure your API Key by routing calls through your own backend service."

The application exposes two endpoints and caches data in memory for 5min (which is Coinmarketcap's update frequency).

Demo of running application: http://netfour.apphb.com/api/coin

## Instruction
Sign up to get API key from https://pro.coinmarketcap.com/

In Startup.cs, in HttpClient configuration, replace value of "X-CMC_PRO_API_KEY" with your own API key.

```c#
client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", "your-own-api-key");
```
In CORS configuration, replace value of allowed origins with your client-side application's url.
e.g.
```c#
services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins",
                    builder => builder
                    .WithOrigins("http://yoururl.com")
```
