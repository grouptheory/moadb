#!/bin/csh -f

set ROOT=`pwd | sed -e 's+\(.*execution_\).*+\1+'`
set CWD=`pwd`

cd $ROOT
if ($# == 1) then
    if (-d $1) then
	echo "metaexp_config_dir"
	cp $1/TemplateDistribution.xml .
	cp $1/DistributionIteratorSpec.xml .
	cp $1/Config.TableGeneration.xml .

	date
	ls -l TemplateDistribution.xml DistributionIteratorSpec.xml Config.TableGeneration.xml 
    else 
	echo "metaexp_config_dir: $1 not found"
    endif
else
    echo "Usage: load.sh metaexp_config_dir"
    exit
endif





