#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
DLDDIR=$TOPDIR/downloads

export PATH=/opt/mono-2.8.2/bin:$PATH
export PKG_CONFIG_PATH=/opt/mono-2.8.2/lib/pkgconfig:$PKG_CONFIG_PATH


echo "updating existing system"
sudo apt-get update
sudo apt-get upgrade -y

echo "installing prerequisites"
sudo apt-get install -y build-essential libc6-dev g++ gcc libglib2.0-dev pkg-config git-core apache2 apache2-threaded-dev bison gettext autoconf automake libtool libpango1.0-dev libatk1.0-dev libgtk2.0-dev libtiff4-dev libgif-dev libglade2-dev

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

wget http://ftp.novell.com/pub/mono/sources/xsp/xsp-2.8.2.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mod_mono/mod_mono-2.8.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mono/mono-2.8.2.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/libgdiplus/libgdiplus-2.8.1.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/gtk-sharp212/gtk-sharp-2.12.10.tar.bz2

cd $BUILDDIR
bunzip2 -df xsp-2.8.2.tar.bz2
tar -xvf xsp-2.8.2.tar

bunzip2 -df mod_mono-2.8.tar.bz2
tar -xvf mod_mono-2.8.tar

bunzip2 -df mono-2.8.2.tar.bz2
tar -xvf mono-2.8.2.tar

bunzip2 -df libgdiplus-2.8.1.tar.bz2
tar -xvf libgdiplus-2.8.1.tar

bunzip2 -df gtk-sharp-2.12.10.tar.bz2
tar -xvf gtk-sharp-2.12.10.tar

echo
echo "building and installing mono packages"
echo


cd $BUILDDIR
cd libgdiplus-2.8.1
./configure --prefix=/opt/mono-2.8.2
make
sudo make install

cd $BUILDDIR
cd mono-2.8.2
./configure --prefix=/opt/mono-2.8.2
make
sudo make install

cd $BUILDDIR
cd gtk-sharp-2.12.10
./configure --prefix=/opt/mono-2.8.2
make
sudo make install

cd $BUILDDIR
cd xsp-2.8.2
./configure --prefix=/opt/mono-2.8.2
make
sudo make install

cd $BUILDDIR
cd mod_mono-2.8
./configure --prefix=/opt/mono-2.8.2
make
sudo make install
cd $BUILDDIR

echo
echo "done"
echo "If you want to use this version as the default on your platform (may not work for everything with this bare install), set your PATH to include /opt/mono-2.8.2/bin. Otherwise for Asp.NET, you can just update your virtual host config to use mod-mono-server2 from the install path directly."
