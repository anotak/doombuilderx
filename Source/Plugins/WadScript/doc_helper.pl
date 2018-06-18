#!/usr/bin/perl -w
# doc_helper.pl by anotak
# this is for use with Doom Builder X in order to help with making documentation

# this code is distributed under the MIT License:

# Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
# The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

use warnings;
use strict;

foreach my $filename (@ARGV)
{
	print $filename . "\n";
	open(INFILE,$filename) or die "$!";
	my @outarray;
	while(<INFILE>)
	{
		if(/\bclass\b/)
		{
			next;
		}
		
		if(/\bstruct\b/)
		{
			next;
		}
		
		if(/\boperator\b/)
		{
			next;
		}
		
		if(/public/)
		{
			s/\bpublic\b//g;
			s/\bfloat\b//g;
			s/\bint\b//g;
			s/\bvoid\b//g;
			s/\bbool\b//g;
			s/\boverride\b//g;
			s/\bstring\b//g;
			s/\bDynValue\b//g;
			s/\bstatic\b//g;
			s/List<.*>//g;
			s/double//g;
			s/Table//g;
			s/\t/ /g;
			s/  / /g;
			s/, /,/g;
			s/\( /(/g;
			
			#clean up extra whitespace chars
			chomp;
			s/^\s*//;
			s/\s*$//;
			push @outarray, "### " . $_ . "\n";
		}
	}
	close(INFILE);
	
	#open(OUTFILE, ">out\\" . $filename . ".outdoc") or die "$!";
	
	foreach (@outarray)
	{
		print;
	}
	
	#close(OUTFILE);
}




