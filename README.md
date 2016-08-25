# A simple project to examine the performance issues of HTTPS usage in Mono on single core devices

## Summary of the issue

Initially, running a simple self-hosting OWIN application that serves HTTP(S) endpoints, we observed (when using HttpS vs Http) the CPU usage on the device spikes up to 100% every 30 seconds, stays up for about 10 seconds and then falls back to about 10% which is the normal amount. The simple application serves an HTML page that makes ajax calls to the REST endpoints exposed by the application and pulls some data every 2-3 seconds. The same application does not behave this way when we switch to HTTP (instead of HTTPS) and there are no spikes. To determine if Mono implementation of HTTPS is the problem or the OWIN/WebAPI, we eliminated OWIN and created this "Simple HTTP listener" that simply listens to an Http(S) connection and serves the endpoints. We again observed the issue. Note that this problem does not occur on a 64 bit desktop Ubuntu machine with 4 cores.

## System Characteristics

- An embedded device running a custom Yocto Linux
- Single core 1.25 GHz 32 bit Atom CPU
- 360 MB of RAM
- Mono Version: 4.2.1.102

## Configuring the application

- You can configure the HTTP(S) port and the protocol (HTTP vs HTTPS) in the webconfig.json file. The host name is irrelevant here.
- If using HTTPS, you need to create a self-signed Server certifiate for the server.
- Also Mono implementation of the HTTPS requests the Client to send a (Any!) Client certificates to the server in order to establish a secure communication. So you need to either create a Client certificate and import it into your browser or use an existing Client Certificate that is already available in your browser.

## Profiling

To observe the spikes, once you run the application you can find the PID of the process using the following command: 
- $ pgrep mono 
and then use that PID in the following command to print out the CPU/Memory usage. You can either write that data to a file for a couple of minutes or simply keep an eye on the numbers to observe the CPU spiking up to 100% every ~30 seconds. 
- $ top -p <PID>
