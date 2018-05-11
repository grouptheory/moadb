#!/bin/csh -f

set CWD=`pwd`
mono ../bin/utils.exe --uniform --variable=optimism --lower=-1 --upper=1 --min=-1 --max=1 --outfile=$CWD/optimism.xml
mono ../bin/utils.exe --uniform --variable=trendiness --lower=-1 --upper=1 --min=-1 --max=1 --outfile=$CWD/trendiness.xml
mono ../bin/utils.exe --uniform --variable=riskiness --lower=0 --upper=2 --min=0 --max=2 --outfile=$CWD/riskiness.xml

mono ../bin/utils.exe --pointed --variable=clock --value=5 --min=2 --max=105 --outfile=$CWD/clock.xml
mono ../bin/utils.exe --pointed --variable=patience --value=0.2 --min=0.01 --max=1 --outfile=$CWD/patience.xml
mono ../bin/utils.exe --pointed --variable=aggressiveness --value=0.5 --min=0 --max=1 --outfile=$CWD/aggressiveness.xml

mono ../bin/utils.exe --product --file1=$CWD/aggressiveness.xml --file2=$CWD/clock.xml --outfile=$CWD/p2.xml
mono ../bin/utils.exe --product --file1=$CWD/p2.xml --file2=$CWD/optimism.xml --outfile=$CWD/p3.xml
mono ../bin/utils.exe --product --file1=$CWD/p3.xml --file2=$CWD/patience.xml --outfile=$CWD/p4.xml
mono ../bin/utils.exe --product --file1=$CWD/p4.xml --file2=$CWD/riskiness.xml --outfile=$CWD/p5.xml
mono ../bin/utils.exe --product --file1=$CWD/p5.xml --file2=$CWD/trendiness.xml --outfile=$CWD/p6.xml
cp p6.xml TemplateDistribution.xml
