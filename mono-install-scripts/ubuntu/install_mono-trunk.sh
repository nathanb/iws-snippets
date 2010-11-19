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

svn co svn://anonsvn.mono-project.com/source/trunk/xsp
svn co svn://anonsvn.mono-project.com/source/trunk/mod_mono

wget http://mono.ximian.com/daily/mono-latest.tar.bz2

cd $BUILDDIR
bunzip2 -df mono-latest.tar.bz2
tar -xvf mono-latest.tar

echo
echo "building and installing mono packages"
echo
cd $BUILDDIR
cd mono-*
./configure --prefix=/usr/local --with-glib=system
make
sudo make install

cd $BUILDDIR
cd xsp
./autogen.sh --prefix=/usr/local
make
sudo make install

cd $BUILDDIR
cd mod_mono
./autogen.sh --prefix=/usr/local
make
sudo make install
cd $BUILDDIR

echo
echo "done"
