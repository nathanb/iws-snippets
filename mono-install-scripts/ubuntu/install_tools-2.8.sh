#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build-tools

export PATH=/opt/mono-2.8/bin:$PATH


echo "updating existing system"
sudo apt-get update
sudo apt-get upgrade -y

echo "installing prerequisites"
sudo apt-get install -y build-essential libc6-dev g++ gcc libglib2.0-dev pkg-config subversion apache2 apache2-threaded-dev bison gettext autoconf automake libtool libpango1.0-dev libatk1.0-dev libgtk2.0-dev libtiff4-dev libgif-dev libglade2-dev libnspr4-dev libwebkit-dev libnss3-dev xulrunner-dev libgnomecanvas2-dev libgnome2-dev libgnomeui-dev

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

wget http://ftp.novell.com/pub/mono/sources/mono-tools/mono-tools-2.8.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/gluezilla/gluezilla-2.6.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/gecko-sharp2/gecko-sharp-2.0-0.13.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mono-debugger/mono-debugger-2.8.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/monodevelop-database/monodevelop-database-2.4.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/monodevelop-debugger-gdb/monodevelop-debugger-gdb-2.4.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/monodevelop-debugger-mdb/monodevelop-debugger-mdb-2.4.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mono-addins/mono-addins-0.5.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/gnome-desktop-sharp2/gnome-desktop-sharp-2.24.0.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/gnome-sharp2/gnome-sharp-2.24.1.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/webkit-sharp/webkit-sharp-0.3.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/monodevelop/monodevelop-2.4.tar.bz2



cd $BUILDDIR

bunzip2 -df *.tar.bz2

tar -xvf monodevelop-2.4.tar
tar -xvf mono-tools-2.8.tar
tar -xvf mono-debugger-2.8.tar
tar -xvf monodevelop-database-2.4.tar
tar -xvf monodevelop-debugger-gdb-2.4.tar
tar -xvf monodevelop-debugger-mdb-2.4.tar
tar -xvf mono-addins-0.5.tar
tar -xvf gnome-desktop-sharp-2.24.0.tar
tar -xvf gnome-sharp-2.24.1.tar
tar -xvf gluezilla-2.6.tar
tar -xvf gecko-sharp-2.0-0.13.tar
tar -xvf webkit-sharp-0.3.tar

echo
echo "building and installing mono packages"
echo

cd $BUILDDIR
cd mono-addins-0.5
./configure --prefix=/opt/mono-2.8
make
make install

cd $BUILDDIR
cd gecko-sharp-2.0-0.13
./configure --prefix=/opt/mono-2.8
make
make install

cd $BUILDDIR
cd webkit-sharp-0.3
./configure --prefix=/opt/mono-2.8
make
make install

cd $BUILDDIR
cd gluezilla-2.6
./configure --prefix=/opt/mono-2.8
make
make install


cd $BUILDDIR
cd gnome-sharp-2.24.1
./configure --prefix=/opt/mono-2.8
make
make install

cd $BUILDDIR
cd mono-tools-2.8
./configure --prefix=/opt/mono-2.8
make
make install

cd $BUILDDIR
cd monodevelop-2.4
./configure --prefix=/opt/mono-2.8
make
make install

cd $BUILDDIR
cd monodevelop-database-2.4
./configure --prefix=/opt/mono-2.8
make
make install

cd $BUILDDIR
cd monodevelop-debugger-gdb
./configure --prefix=/opt/mono-2.8
make
make install

cd $BUILDDIR
cd monodevelop-debugger-mdb
./configure --prefix=/opt/mono-2.8
make
make install


cd $BUILDDIR
echo
echo "done"
