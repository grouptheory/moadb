#!/bin/csh -f

set CWD=`pwd`

# Clock
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=Clock --value=15 --min=1 --max=100 --outfile=$CWD/Clock1.xml
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=Clock --value=50 --min=1 --max=100 --outfile=$CWD/Clock2.xml

mono ../../utils/bin/Debug/utils.exe --mixtureimmutable --weight1=0.90 --file1=$CWD/Clock1.xml --file2=$CWD/Clock2.xml --outfile=$CWD/MixClock.xml


mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=Clock --value=10 --min=1 --max=100 --outfile=$CWD/Clock1prime.xml
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=Clock --value=2 --min=1 --max=100 --outfile=$CWD/Clock2prime.xml

mono ../../utils/bin/Debug/utils.exe --mixtureimmutable --weight1=0.90 --file1=$CWD/Clock1prime.xml --file2=$CWD/Clock2prime.xml --outfile=$CWD/MixClockprime.xml


mono ../../utils/bin/Debug/utils.exe --mixture --weight1=0.50 --file1=$CWD/MixClock.xml --file2=$CWD/MixClockprime.xml --outfile=$CWD/DM2PL_Clock.xml

# PriceOffset
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=PriceOffset --value=0.01 --min=-1 --max=1 --outfile=$CWD/PriceOffset1.xml
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=PriceOffset --value=0.05 --min=-1 --max=1 --outfile=$CWD/PriceOffset2.xml

mono ../../utils/bin/Debug/utils.exe --mixtureimmutable --weight1=0.90 --file1=$CWD/PriceOffset1.xml --file2=$CWD/PriceOffset2.xml --outfile=$CWD/MixPriceOffset.xml


mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=PriceOffset --value=-0.01 --min=-1 --max=1 --outfile=$CWD/PriceOffset1prime.xml
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=PriceOffset --value=-0.05 --min=-1 --max=1 --outfile=$CWD/PriceOffset2prime.xml

mono ../../utils/bin/Debug/utils.exe --mixtureimmutable --weight1=0.90 --file1=$CWD/PriceOffset1prime.xml --file2=$CWD/PriceOffset2prime.xml --outfile=$CWD/MixPriceOffsetprime.xml


mono ../../utils/bin/Debug/utils.exe --mixture --weight1=0.50 --file1=$CWD/MixPriceOffset.xml --file2=$CWD/MixPriceOffsetprime.xml --outfile=$CWD/DM2PL_PriceOffset.xml

# VolumeOffset
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=VolumeOffset --value=150 --min=-1 --max=1000 --outfile=$CWD/VolumeOffset1.xml
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=VolumeOffset --value=900 --min=-1 --max=1000 --outfile=$CWD/VolumeOffset2.xml

mono ../../utils/bin/Debug/utils.exe --mixtureimmutable --weight1=0.90 --file1=$CWD/VolumeOffset1.xml --file2=$CWD/VolumeOffset2.xml --outfile=$CWD/MixVolumeOffset.xml


mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=VolumeOffset --value=50 --min=-1 --max=1000 --outfile=$CWD/VolumeOffset1prime.xml
mono ../../utils/bin/Debug/utils.exe --pointedimmutable --variable=VolumeOffset --value=0 --min=-1 --max=1000 --outfile=$CWD/VolumeOffset2prime.xml

mono ../../utils/bin/Debug/utils.exe --mixtureimmutable --weight1=0.90 --file1=$CWD/VolumeOffset1prime.xml --file2=$CWD/VolumeOffset2prime.xml --outfile=$CWD/MixVolumeOffsetprime.xml


mono ../../utils/bin/Debug/utils.exe --mixture --weight1=0.50 --file1=$CWD/MixVolumeOffset.xml --file2=$CWD/MixVolumeOffsetprime.xml --outfile=$CWD/DM2PL_VolumeOffset.xml

# Producting

mono ../../utils/bin/Debug/utils.exe --product --file1=$CWD/DM2PL_Clock.xml --file2=$CWD/DM2PL_PriceOffset.xml --outfile=$CWD/p2.xml
mono ../../utils/bin/Debug/utils.exe --product --file1=$CWD/DM2PL_VolumeOffset.xml --file2=$CWD/p2.xml --outfile=$CWD/p3.xml


cp p3.xml TemplateDistribution.xml

