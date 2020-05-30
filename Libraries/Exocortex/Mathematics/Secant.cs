// Exocortex Technologies
// http://www.exocortex.org
// Copyright (c) 2001, 2002 Ben Houston (ben@exocortex.org).  All Rights Reserved.

using System;

namespace Exocortex.Mathematics
{
	
	// The secant method is one of the simplest methods for solving algebraic
	// equations. It is usually used as a part of a larger algorithm to improve
	// convergence. As in any numerical algorithm, we need to check that the
	// method is converging to a given precision in a certain number of steps.
	// This is a precaution to avoid an infinite loop.
	class Secant {

		public delegate double Function(double x); //declare a delegate that takes double and returns double

		public static void secant(int step_number, double point1,double point2,Function f) {
			double p2,p1,p0,prec=.0001f; //set precision to .0001
			int i;
			p0=f(point1);
			p1=f(point2);
			p2=p1-f(p1)*(p1-p0)/(f(p1)-f(p0)); //secant formula

			for(i=0;System.Math.Abs(p2)>prec &&i<step_number;i++) { //iterate till precision goal is not met or the maximum //number of steps is reached
				p0=p1;
				p1=p2;
				p2=p1-f(p1)*(p1-p0)/(f(p1)-f(p0));
			}
			if(i<step_number)
				Console.WriteLine(p2); //method converges
			else
				Console.WriteLine("{0}.The method did not converge",p2);//method does not converge
		}
	}

}
