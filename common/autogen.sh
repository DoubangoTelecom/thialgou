##### CSharp
echo "--->CSharp...<---"
swig -DCOMMON_API -DCOMMON_INLINE -c++ -csharp -namespace org.doubango.thialgou.commonWRAP -outdir csharp -o csharp/commonWRAP.cc swig/csharp.i

##### Objective-C
#echo "--->Objective-C...<---"
#swig -c++ -objc -outdir objc -o -objc/commonWRAP.cc swig/objc.i

##### Java
echo "--->Java...<---"
#swig -c++ -java -package org.doubango.thialgou.ioWRAP -outdir java -o java/commonWRAP.cc swig/java.i

##### Python
echo "--->Python...<---"
#swig -c++ -python -outdir python -o python/ioWRAP.cc swig/python.i

##### Perl
echo "--->Perl...<---"
#swig -c++ -perl -outdir perl -o Perl/ioWRAP.cc swig/perl.i

##### Ruby
echo "--->Ruby...<---"
#swig -c++ -ruby -outdir ruby -o Ruby/ioWRAP.cc swig/ruby.i