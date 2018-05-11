#!/bin/csh -f

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

if ($# != 0) then
    echo "Usage: gp.sh"
    exit
endif

cd frames

foreach F (`ls *.gp`)
   cat $F | gnuplot
end

gs -dBATCH -dNOPAUSE -q -sDEVICE=pdfwrite -sOutputFile=Orderbook.pdf *.eps


