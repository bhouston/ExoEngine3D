// Exocortex Technologies
// http://www.exocortex.org
// Copyright (c) 2001, 2002 Ben Houston (ben@exocortex.org).  All Rights Reserved.

using System;

namespace Exocortex.Mathematics
{
	//Simpson integration algorithm
	public class SimpsonIntegral {

		//calculate the integral of f(x) between x=a and x=b by spliting the interval in step_number steps

		public delegate double Function(double x); //declare a delegate that takes and returns double 
		public static double integral(Function f,double a, double b,int step_number) {
			double sum=0;
			double step_size=(b-a)/step_number;
			for(int i=0;i<step_number;i=i+2) //Simpson algorithm samples the integrand in several point which significantly improves //precision.
				sum=sum+(f(a+i*step_size)+4*f(a+(i+1)*step_size)+f(a+(i+2)*step_size))*step_size/3; //divide the area under f(x) //into step_number rectangles and sum their areas 
			return sum;
		}

	}
}
