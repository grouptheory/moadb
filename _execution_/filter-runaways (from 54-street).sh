#!/bin/tcsh -f

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

if ($# != 2) then
    echo "Usage: filter-runaways.sh PAGE NAME"
    exit
endif

set PAGE=$1
set OUT=$2

# 

echo "Scanning experiment directories, please wait"

set SCRIPTOUT="PRUNED.sh"

rm -f $SCRIPTOUT
touch $SCRIPTOUT
echo "#\!/bin/csh -f" >> $SCRIPTOUT

#set PAGE=15
#set OUT="Price-Extracted"

# foreach F (`ls -d experiment.*`)
foreach F (`find . -name 'experiment.*' | sed -e 's+./++'`)
    if (-f $F/trajectories/Price-1.tra) then
	set GOOD=`../filter.sh $F | grep Good | wc -l | sed -e 's/  *//'`
	if ($GOOD == 1) then
	    echo "evince $F/results/report-*.pdf"
	    echo "gs -sDEVICE=pdfwrite -dNOPAUSE -dBATCH -dSAFER -dFirstPage=$PAGE -dLastPage=$PAGE -sOutputFile=$F/$OUT.pdf $F/results/report-*.pdf" >> $SCRIPTOUT
	else 
	    echo "RUNAWAY: $F"
	endif
    endif
end

    
echo "gs -dBATCH -dNOPAUSE -q -sDEVICE=pdfwrite -sOutputFile=$OUT-book.pdf experiment.*/$OUT.pdf" >> $SCRIPTOUT

chmod 755 $SCRIPTOUT

# run the PRUNED script
./$SCRIPTOUT

evince $OUT-book.pdf


