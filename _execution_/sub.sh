#!/bin/csh -f

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

if ($# != 0) then
    echo "Usage: sub.sh"
    exit
endif

foreach F (`ls -d experiment.*`)
    echo "$F starting."
    ../run.sh $F
    echo "$F finished."
end


