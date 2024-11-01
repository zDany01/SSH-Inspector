# SSH Inspector

A simple tool to analyze your SSH login logs and filter authentication attempts.
It will also extrapolate the IP Address and create a list with all the IP Address information

![demo](https://github.com/zDany01/zDany01/blob/main/Assets/SSH%20Inspector/demo.png?raw=true)
## Usage
To use the program you need to get your ssh service log using using this command
```bash
journalctl -u ssh > log.txt
```
> Depending on your system configuration you may also need to execute the previous command as root
Then call the program with the saved log file
```bash
sshinsp <logfilepath>
```