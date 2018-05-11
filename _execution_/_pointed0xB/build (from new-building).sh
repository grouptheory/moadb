#!/bin/csh -f

set CWD=`pwd`
mono ../bin/utils.exe --pointed --variable=Clock --value=30 --min=2 --max=100 --outfile=$CWD/Clock-1.xml
mono ../bin/utils.exe --pointed --variable=GainCutoff --value=0.05 --min=0.00 --max=0.06 --outfile=$CWD/GainCutoff-1.xml
mono ../bin/utils.exe --pointed --variable=LossCutoff --value=0.05 --min=0.00 --max=0.06 --outfile=$CWD/LossCutoff-1.xml
mono ../bin/utils.exe --pointed --variable=AgentType --value=0.5 --min=0.0 --max=1.0 --outfile=$CWD/AgentType-1.xml

mono ../bin/utils.exe --product --file1=$CWD/Clock-1.xml --file2=$CWD/GainCutoff-1.xml --outfile=$CWD/p2-1.xml
mono ../bin/utils.exe --product --file1=$CWD/p2-1.xml --file2=$CWD/LossCutoff-1.xml --outfile=$CWD/p3-1.xml
mono ../bin/utils.exe --product --file1=$CWD/p3-1.xml --file2=$CWD/AgentType-1.xml --outfile=$CWD/p4-1.xml

mono ../bin/utils.exe --pointed --variable=Clock --value=30 --min=2 --max=100 --outfile=$CWD/Clock-2.xml
mono ../bin/utils.exe --pointed --variable=GainCutoff --value=0.05 --min=0.00 --max=0.06 --outfile=$CWD/GainCutoff-2.xml
mono ../bin/utils.exe --pointed --variable=LossCutoff --value=0.05 --min=0.00 --max=0.06 --outfile=$CWD/LossCutoff-2.xml
mono ../bin/utils.exe --pointed --variable=AgentType --value=0.5 --min=0.0 --max=1.0 --outfile=$CWD/AgentType-2.xml

mono ../bin/utils.exe --product --file1=$CWD/Clock-2.xml --file2=$CWD/GainCutoff-2.xml --outfile=$CWD/p2-2.xml
mono ../bin/utils.exe --product --file1=$CWD/p2-2.xml --file2=$CWD/LossCutoff-2.xml --outfile=$CWD/p3-2.xml
mono ../bin/utils.exe --product --file1=$CWD/p3-2.xml --file2=$CWD/AgentType-2.xml --outfile=$CWD/p4-2.xml

mono ../bin/utils.exe --mixture --weight1=0.5 --file1=$CWD/p4-1.xml --file2=$CWD/p4-2.xml --outfile=$CWD/pMix.xml

cp pMix.xml TemplateDistribution.xml
