#!/bin/csh -f

set CWD=`pwd`
mono ../bin/utils.exe --pointed --variable=Clock --value=60 --min=2 --max=2000 --outfile=$CWD/Clock.xml
mono ../bin/utils.exe --pointed --variable=GainCutoff --value=1.01 --min=0.00 --max=10.00 --outfile=$CWD/GainCutoff.xml
mono ../bin/utils.exe --pointed --variable=LossCutoff --value=1.01 --min=0.00 --max=10.00 --outfile=$CWD/LossCutoff.xml
mono ../bin/utils.exe --uniform --variable=AgentType --lower=-0.9 --upper=1.1 --min=-2 --max=2 --outfile=$CWD/AgentType.xml

mono ../bin/utils.exe --product --file1=$CWD/Clock.xml --file2=$CWD/GainCutoff.xml --outfile=$CWD/p2.xml
mono ../bin/utils.exe --product --file1=$CWD/p2.xml --file2=$CWD/LossCutoff.xml --outfile=$CWD/p3.xml
mono ../bin/utils.exe --product --file1=$CWD/p3.xml --file2=$CWD/AgentType.xml --outfile=$CWD/p4.xml
cp p4.xml TemplateDistribution.xml
