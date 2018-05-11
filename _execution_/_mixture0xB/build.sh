#!/bin/csh -f

set CWD=`pwd`

# everyone has a very long clock
mono ../bin/utils.exe --pointed --variable=Clock --value=10000 --min=2 --max=10000 --outfile=$CWD/Clock.xml

# mix long w short in 0.2 : 2.0 ratio
mono ../bin/utils.exe --uniform --variable=AgentType --lower=-0.9 --upper=1.1 --min=-10 --max=10 --outfile=$CWD/AgentType.xml

# timid
mono ../bin/utils.exe --pointed --variable=GainCutoff --value=0.01 --min=0.00 --max=10.00 --outfile=$CWD/GainCutoffTimid.xml
mono ../bin/utils.exe --pointed --variable=LossCutoff --value=0.01 --min=0.00 --max=10.00 --outfile=$CWD/LossCutoffTimid.xml
mono ../bin/utils.exe --product --file1=$CWD/GainCutoffTimid.xml --file2=$CWD/LossCutoffTimid.xml --outfile=$CWD/CutoffTimid.xml

# risky
mono ../bin/utils.exe --pointed --variable=GainCutoff --value=0.25 --min=0.00 --max=10.00 --outfile=$CWD/GainCutoffRisky.xml
mono ../bin/utils.exe --pointed --variable=LossCutoff --value=0.25 --min=0.00 --max=10.00 --outfile=$CWD/LossCutoffRisky.xml
mono ../bin/utils.exe --product --file1=$CWD/GainCutoffRisky.xml --file2=$CWD/LossCutoffRisky.xml --outfile=$CWD/CutoffRisky.xml

# mix risky with timid in 2:1 ratio
mono ../bin/utils.exe --mixture --file1=$CWD/CutoffRisky.xml --weight1=0.55 --file2=$CWD/CutoffTimid.xml --outfile=$CWD/PriceCutoff.xml

mono ../bin/utils.exe --product --file1=$CWD/Clock.xml --file2=$CWD/PriceCutoff.xml --outfile=$CWD/p2.xml
mono ../bin/utils.exe --product --file1=$CWD/p2.xml --file2=$CWD/AgentType.xml --outfile=$CWD/p3.xml

cp p3.xml TemplateDistribution.xml
