#!/bin/csh -f

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

cd $ROOT

rm -rf metaexperiment.*
rm -rf experiment.*
rm *~


