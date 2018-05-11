#!/bin/csh -f

set CWD=`pwd`

mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=A --value=2 --min=1 --max=100 --outfile=$CWD/A1.xml
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=A --value=99 --min=1 --max=100 --outfile=$CWD/A2.xml

mono ../../utils/bin/Debug/utils.exe --mixtureimmutable --weight1=0.90 --file1=$CWD/A1.xml --file2=$CWD/A2.xml --outfile=$CWD/PL1.xml


mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=A --value=5 --min=1 --max=100 --outfile=$CWD/A1prime.xml
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=A --value=95 --min=1 --max=100 --outfile=$CWD/A2prime.xml

mono ../../utils/bin/Debug/utils.exe --mixtureimmutable --weight1=0.90 --file1=$CWD/A1prime.xml --file2=$CWD/A2prime.xml --outfile=$CWD/PL2.xml


mono ../../utils/bin/Debug/utils.exe --mixture --weight1=0.50 --file1=$CWD/PL1.xml --file2=$CWD/PL2.xml --outfile=$CWD/DM2PL.xml

cp DM2PL.xml TemplateDistribution.xml

