#!/bin/csh -f

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

if ($# != 1) then
    echo "Usage: run.sh experiment-name"
    exit
endif

cp $ROOT/../experiment/bin/Debug/* $ROOT/bin/
mono $ROOT/bin/experiment.exe $1

