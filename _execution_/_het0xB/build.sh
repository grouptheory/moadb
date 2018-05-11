#!/bin/csh -f

set CWD=`pwd`

# timid
mono ../bin/utils.exe --pointed --variable=GainCutoff --value=0.01 --min=0.00 --max=10.00 --outfile=$CWD/GainCutoffTimid.xml
mono ../bin/utils.exe --pointed --variable=LossCutoff --value=0.01 --min=0.00 --max=10.00 --outfile=$CWD/LossCutoffTimid.xml
mono ../bin/utils.exe --product --file1=$CWD/GainCutoffTimid.xml --file2=$CWD/LossCutoffTimid.xml --outfile=$CWD/CutoffTimid.xml

# risky
mono ../bin/utils.exe --pointed --variable=GainCutoff --value=0.25 --min=0.00 --max=10.00 --outfile=$CWD/GainCutoffRisky.xml
mono ../bin/utils.exe --pointed --variable=LossCutoff --value=0.25 --min=0.00 --max=10.00 --outfile=$CWD/LossCutoffRisky.xml
mono ../bin/utils.exe --product --file1=$CWD/GainCutoffRisky.xml --file2=$CWD/LossCutoffRisky.xml --outfile=$CWD/CutoffRisky.xml


# mostlytimid
mono ../bin/utils.exe --mixture --file1=$CWD/CutoffRisky.xml --weight1=0.4 --file2=$CWD/CutoffTimid.xml --outfile=$CWD/MostlyTimid.xml
# pure long
mono ../bin/utils.exe --pointed --variable=AgentType --value=+1 --min=-10 --max=10 --outfile=$CWD/AgentTypeLong.xml
mono ../bin/utils.exe --product --file1=$CWD/AgentTypeLong.xml --file2=$CWD/MostlyTimid.xml --outfile=$CWD/LongTimids.xml




# mostlyrisky
mono ../bin/utils.exe --mixture --file1=$CWD/CutoffRisky.xml --weight1=0.6 --file2=$CWD/CutoffTimid.xml --outfile=$CWD/MostlyRisky.xml
# pure short
mono ../bin/utils.exe --pointed --variable=AgentType --value=-1 --min=-10 --max=10 --outfile=$CWD/AgentTypeShort.xml
mono ../bin/utils.exe --product --file1=$CWD/AgentTypeShort.xml --file2=$CWD/MostlyRisky.xml --outfile=$CWD/ShortRiskys.xml


# mix long timids with short riskys
mono ../bin/utils.exe --mixture --file1=$CWD/LongTimids.xml --weight1=0.5 --file2=$CWD/ShortRiskys.xml --outfile=$CWD/p2.xml

mono ../bin/utils.exe --pointed --variable=Clock --value=10000 --min=2 --max=10000 --outfile=$CWD/Clock.xml

mono ../bin/utils.exe --product --file1=$CWD/Clock.xml --file2=$CWD/p2.xml --outfile=$CWD/p3.xml

cp p3.xml TemplateDistribution.xml