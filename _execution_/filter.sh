#!/bin/csh -f

set EXPNAME = $1
set FILE = "$EXPNAME/trajectories/Price-1.tra"

set FINALPRICE = `tail -1 $FILE | tr '\t' ' '  | cut -f 2 -d ' '`

set ISEXP = `tail -1 $FILE | tr '\t' ' '  | cut -f 2 -d ' ' | grep E | wc -l | sed -e 's/  *//'`

set LOW = 5
set HIGH = 15

#echo "ISEXP = $ISEXP"
#echo "FINALPRICE = $FINALPRICE"

if ($ISEXP == 1) then 
#echo "$FILE is Exp"
else

set notbiggerthanhigh=`echo $FINALPRICE - $HIGH| bc |grep "-" |wc -l`
set notlessthanlow=`echo $LOW - $FINALPRICE| bc |grep "-" |wc -l`

if ($notbiggerthanhigh == 1) then
    if ($notlessthanlow == 1) then
	echo "$FILE is Good"
    else 
#	echo "$FILE is Low"
    endif
else
#    echo "$FILE is High"
endif

endif
