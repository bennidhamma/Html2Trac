#!/bin/sh
cd /home/ben/Projects/Html2Trac
./html2xhtml-1.1.2-2/src/html2xhtml --compact-block-elements -l 10000 $1 | ./Html2Trac/bin/Debug/Html2Trac.exe -n $2 -t

