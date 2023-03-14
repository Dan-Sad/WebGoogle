# Description
The app allows you to manage your personal "blog" using Google authorization. The app consists of a Web API and a UI based on Razor Page.

# Configuration
To add the "appsettings.json" file and add the following code to the file:

``` 
"Authentication": {
    "Google": {
      "ClientId": YOUR-CLIENT-ID,
      "ClientSecret": YOUR-CLIENT-SECRET
    }
  },
  "Blogs": YOUR-BLOGS-JSON-FILE
```
These settings are required for proper authentication and to link your Google account with the application. Make sure to replace the "ClientId" and "ClientSecret" values with your own values obtained from the Google API Console.

# UI
![image](https://user-images.githubusercontent.com/78426216/224829010-efc76799-ffa7-466f-857e-288e6a2121bb.png)
