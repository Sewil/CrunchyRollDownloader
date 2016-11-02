# How to use
Crunchyrolldownloader [OPTIONS]

Options:
 -u [ARG]: The URL to download from
 -b [ARG]: Link to batch text file, with one URL per row.
 -s:       Skips video download and only extracts english subtitles, if there are any.
 
 # Dependecies
- Relies on youtube-dl which in turn relies on rtmpdump. The application downloads rtmpdump automatically if none is set in PATH. In the release I have attached youtube-dl so just set your PATH to the application folder and run it.

# Configuration
- The default format for download is "[height=720p]" so if you're not a premium member it'll fail to download. To change this, log in and close the application, then go to AppData/Local/CrunchyRollDownloader and open the .config file. Add a new <setting /> named "f" within <CrunchyRollDownloader.User /> and enter your desired format.
