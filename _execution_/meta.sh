#!/bin/csh -fx

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

if ($# != 1) then
    echo "Usage: meta.sh experiment-name"
    exit
endif

cd $ROOT
cp $ROOT/../metaexperiment/bin/Debug/* $ROOT/bin/
mono $ROOT/bin/metaexperiment.exe $1

