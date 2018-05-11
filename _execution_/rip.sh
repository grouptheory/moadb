#!/bin/csh -f

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

if ($# != 0) then
    echo "Usage: sub.sh"
    exit
endif

set PAGE=22
set OUT="spread"

foreach F (`ls -d experiment.*`)
    echo "extracting from $F"
    gs -sDEVICE=pdfwrite -dNOPAUSE -dBATCH -dSAFER -dFirstPage=$PAGE -dLastPage=$PAGE -sOutputFile=$F/$OUT.pdf $F/results/report-*.pdf
end

gs -dBATCH -dNOPAUSE -q -sDEVICE=pdfwrite -sOutputFile=$OUT-book.pdf experiment.*/$OUT.pdf

