#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
DLDDIR=$TOPDIR/downloads

export PATH=/usr/local/bin:$PATH

echo "updating existing system"
sudo apt-get update
sudo apt-get upgrade -y

echo "installing prerequisites"
sudo apt-get install -y build-essential libc6-dev g++ gcc libglib2.0-dev pkg-config subversion apache2 apache2-threaded-dev bison gettext autoconf automake libtool

mkdir -p $BUILDDIR


echo
echo "downloading mono packages"
echo

cd $BUILDDIR

wget http://ftp.novell.com/pub/mono/sources/mono/mono-2.6.4.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/libgdiplus/libgdiplus-2.6.4.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/xsp/xsp-2.6.4.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mod_mono/mod_mono-2.6.3.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mono-debugger/mono-debugger-2.6.3.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mono-tools/mono-tools-2.6.2.tar.bz2

bunzip2 -df mono-2.6.4.tar.bz2
tar -xvf mono-2.6.4.tar

bunzip2 -df libgdiplus-2.6.4.tar.bz2
tar -xvf libgdiplus-2.6.4.tar

bunzip2 -df xsp-2.6.4.tar.bz2
tar -xvf xsp-2.6.4.tar

bunzip2 -df mod_mono-2.6.3.tar.bz2
tar -xvf mod_mono-2.6.3.tar

bunzip2 -df mono-debugger-2.6.3.tar.bz2
tar -xvf mono-debugger-2.6.3.tar

bunzip2 -df mono-tools-2.6.2.tar.bz2
tar -xvf mono-tools-2.6.2.tar

echo
echo "building and installing mono packages"
echo
cd $BUILDDIR

cd libgdiplus*
./configure.sh --prefix=/usr/local
make
sudo make install

cd $BUILDDIR
cd mono*
./configure.sh --prefix=/usr/local
make
sudo make install
cd $BUILDDIR

cd mono-tools*
./configure.sh --prefix=/usr/local
make
sudo make install
cd $BUILDDIR

cd mono-debugger*
./configure.sh --prefix=/usr/local
make
sudo make install
cd $BUILDDIR

cd xsp*
./configure.sh --prefix=/usr/local
make
sudo make install
cd $BUILDDIR

cd mod_mono*
./configure.sh --prefix=/usr/local
make
sudo make install
cd $BUILDDIR

echo
echo "done"
