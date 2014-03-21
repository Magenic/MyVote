@echo off

SET CHROME_BIN=C:\Program Files (x86)\Google\Chrome\Application\chrome.exe
SET FIREFOX_BIN=C:\Program Files (x86)\Mozilla Firefox\firefox.exe

karma start "%~dp0\controllers_continuous.conf.js"