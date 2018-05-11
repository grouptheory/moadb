#!/bin/csh -f

set X=1
while ($X < 1798) 
  set BC = `wc -l BIDS-pop-0-trial-0-$X.dat | sed -e 's/ *//' | sed -e 's/ .*//'`
  set AC = `wc -l ASKS-pop-0-trial-0-$X.dat | sed -e 's/ *//' | sed -e 's/ .*//'`
  echo "$X    $BC    $AC"
  @ X = $X + 1
end
