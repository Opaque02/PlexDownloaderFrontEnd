# PlexDownloaderFrontEnd

Issue with getting the API part to work, even though it works when the same code is used in a console app version

If you put a breakpoint at line 96 and 97, the one at 96 gets hit, the one at 97 does not. 

If you do step into from line 96, you'll eventually hit a file called ApiService.cs; in that in line 69, you'll see this line of code:

```            var httpResponse = await this.httpClient.SendAsync(httpRequestMessage);```

As far as I can tell, this is the furtherest it goes - after this point it just hangs and doesn't respond to anything, as if it's waiting for a response and never getting it
