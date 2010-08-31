#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
DLDDIR=$TOPDIR/downloads

export PATH=/usr/local/bin:$PATH

cd $BUILDDIR

echo "building and installing mono packages"
echo
cd $BUILDDIR

cd mod_mono
sudo make uninstall

cd $BUILDDIR
cd xsp
sudo make uninstall

cd $BUILDDIR
cd mono-*
sudo make uninstall

cd $BUILDDIR
rm -rf mono-*

wget http://mono.ximian.com/daily/mono-20100801.tar.bz2

bunzip2 -df mono-20100801.tar.bz2
tar -xvf mono-20100801.tar

cd $BUILDDIR
cd mono-*
./configure --prefix=/usr/local --with-glib=system
make
sudo make install

cd $BUILDDIR
cd xsp
svn update
./autogen.sh --prefix=/usr/local
make
sudo make install

cd $BUILDDIR
cd mod_mono
svn update
./autogen.sh --prefix=/usr/local
make
sudo make install
cd $BUILDDIR


echo
echo "done"