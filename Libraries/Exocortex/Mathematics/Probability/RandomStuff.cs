		private double Uniform_Distribution(int lower, int upper)
		//returns a uniformly distributed double number between lower and upper
		{
			//check parameters for valid input
			if((upper < 0) || (upper <= lower))
			{
				return -1;
			}
			else if (upper == lower)//trivial case
			{
				return upper;
			}

			//generate and return random number within bounds
			double Random_Number = new Double();
			Random r = new Random();
	
			//This is used to help the seeding
			for (int dummycount =0; dummycount < 1000; dummycount++)
				r = new Random();
			
			Random_Number= r.NextDouble();
			return(lower + (Random_Number * (upper - lower)));
		}

		private double Exponential_Distribution(double mean)
			//returns an exponentially distributed double number between lower and upper
		{
			//check parameter for valid input
			if(mean < 0)
			{
				return -1;
			}
			
			//generate and return random number within bounds
			double Random_Number = new Double();
			Random r = new Random();
	
			//This is used to help the seeding
			for (int dummycount =0; dummycount < 1000; dummycount++)
				r = new Random();
			
			Random_Number= 0;
			while (Random_Number == 0)
			{
				Random_Number = r.NextDouble();
			}
			double Exp_Random_Number = new Double();
			Exp_Random_Number= -1 * Math.Log(1 - Random_Number) * mean;
			return(Exp_Random_Number);
		}

		private double rand_stdnormal()
			//return a real random number with standard normal distributions, uses the trick that if you generate 10 or more numbers with uniform distributions and add them together you get a normal number, then it standardizes it
		{
			int samplesize=10; //number of uniformly distributed random variable to sum.
			double [] uniform_random_numbers = new Double[samplesize]; //holds an array of the random variables
			for(int i=0; i<samplesize; i++) //get the random variables
			{
				//generate and return random number within bounds
				double Random_Number = new Double();
				Random r = new Random();
	
				//This is used to help the seeding
				for (int dummycount =0; dummycount < 1000; dummycount++)
					r = new Random();

				uniform_random_numbers[i] = r.NextDouble();
			}
			double sum_uniform=0; //holds sum of random variables
			for(int i=0; i<samplesize; i++) //sum the random variables
			{
				sum_uniform = sum_uniform + uniform_random_numbers[i];
			}
			double nu= samplesize * 0.5; //mean is samplesize times * (mean of uniform=0.5)  
			double ru = Math.Sqrt(1.0/12.0) * Math.Sqrt(samplesize); //standard deviation is sqrt((1-0)^2/12)=1/12
			
			double stdnormal_random_number = 0; //holds standard normally distributed number
			stdnormal_random_number= (sum_uniform - nu) / ru; //central limit theorem
			return (stdnormal_random_number);
		}

		private double mean_finder (double [] numberlist)
		//calculate the mean of the numbers in the numberlist
		{
			double sum=0;
			for (int i=0; i<numberlist.Length; i++)
			{
				sum = sum + numberlist[i];
			} 
			return(sum / numberlist.Length);  
		}

		private double variance_finder (double [] numberlist, double mean)
		//calculate the variance of the numbers in the numberlist
		{
			double squared_sum=0;
			for (int i=0; i<numberlist.Length; i++)
			{
				squared_sum = squared_sum + (numberlist[i] * numberlist[i]);
			}
			return ((squared_sum / numberlist.Length) - (mean * mean));
		}

