#!/bin/csh -f

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

if ($# == 0) then
    if (-f "TemplateDistribution.xml") then
	echo "Distribution file: TemplateDistribution.xml"
	mono $ROOT/bin/utils.exe --print --file1=$CWD/TemplateDistribution.xml
    endif

    if (-f "ActualDistribution.xml") then
	echo "Distribution file: ActualDistribution.xml"
	mono $ROOT/bin/utils.exe --print --file1=$CWD/ActualDistribution.xml
    endif
endif

if ($# == 1) then
    if (-f $1) then
	echo "Distribution file: $1"
	mono $ROOT/bin/utils.exe --print --file1=$CWD/$1
    else 
	echo "Distribution file: $1 not found"
    endif
endif

if ($# > 1) then
    echo "Usage: dist.sh distribution.xml"
    exit
endif





