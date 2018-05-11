#!/bin/csh -f

set CWD=`pwd`
mono ../bin/utils.exe --pointed --variable=clock --value=5 --min=2 --max=105 --outfile=$CWD/clock.xml
mono ../bin/utils.exe --pointed --variable=optimism --value=0.001 --min=-1 --max=1 --outfile=$CWD/optimism.xml
mono ../bin/utils.exe --pointed --variable=patience --value=0.2 --min=0.01 --max=1 --outfile=$CWD/patience.xml
mono ../bin/utils.exe --pointed --variable=trendiness --value=0.1 --min=-1 --max=1 --outfile=$CWD/trendiness.xml
mono ../bin/utils.exe --pointed --variable=aggressiveness --value=0.5 --min=0 --max=1 --outfile=$CWD/aggressiveness.xml
mono ../bin/utils.exe --pointed --variable=riskiness --value=1 --min=0 --max=2 --outfile=$CWD/riskiness.xml

mono ../bin/utils.exe --product --file1=$CWD/aggressiveness.xml --file2=$CWD/clock.xml --outfile=$CWD/p2.xml
mono ../bin/utils.exe --product --file1=$CWD/p2.xml --file2=$CWD/optimism.xml --outfile=$CWD/p3.xml
mono ../bin/utils.exe --product --file1=$CWD/p3.xml --file2=$CWD/patience.xml --outfile=$CWD/p4.xml
mono ../bin/utils.exe --product --file1=$CWD/p4.xml --file2=$CWD/riskiness.xml --outfile=$CWD/p5.xml
mono ../bin/utils.exe --product --file1=$CWD/p5.xml --file2=$CWD/trendiness.xml --outfile=$CWD/p6.xml


mono ../bin/utils.exe --pointed --variable=clock --value=5 --min=2 --max=105 --outfile=$CWD/clock2.xml
mono ../bin/utils.exe --pointed --variable=optimism --value=0.001 --min=-1 --max=1 --outfile=$CWD/optimism2.xml
mono ../bin/utils.exe --pointed --variable=patience --value=0.2 --min=0.01 --max=1 --outfile=$CWD/patience2.xml
mono ../bin/utils.exe --pointed --variable=trendiness --value=0.1 --min=-1 --max=1 --outfile=$CWD/trendiness2.xml
mono ../bin/utils.exe --pointed --variable=aggressiveness --value=0.5 --min=0 --max=1 --outfile=$CWD/aggressiveness2.xml
mono ../bin/utils.exe --pointed --variable=riskiness --value=1 --min=0 --max=2 --outfile=$CWD/riskiness2.xml

mono ../bin/utils.exe --product --file1=$CWD/aggressiveness2.xml --file2=$CWD/clock2.xml --outfile=$CWD/p22.xml
mono ../bin/utils.exe --product --file1=$CWD/p22.xml --file2=$CWD/optimism2.xml --outfile=$CWD/p32.xml
mono ../bin/utils.exe --product --file1=$CWD/p32.xml --file2=$CWD/patience2.xml --outfile=$CWD/p42.xml
mono ../bin/utils.exe --product --file1=$CWD/p42.xml --file2=$CWD/riskiness2.xml --outfile=$CWD/p52.xml
mono ../bin/utils.exe --product --file1=$CWD/p52.xml --file2=$CWD/trendiness2.xml --outfile=$CWD/p62.xml

mono ../bin/utils.exe --mixture --weight1=0.5 --file1=$CWD/p6.xml --file2=$CWD/p62.xml --outfile=$CWD/pMix.xml

cp pMix.xml TemplateDistribution.xml
