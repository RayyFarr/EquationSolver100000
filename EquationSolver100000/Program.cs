using System;
using System.Collections.Generic;
using System.Linq;

namespace EquationSolver9001
{
	internal class Program
	{
		static Equation Eq1;
		static Equation Eq2;
		static bool detailed;
		static ConsoleColor inputColor = ConsoleColor.Green;
		static ConsoleColor outputColor = ConsoleColor.DarkCyan;
		static ConsoleColor resultColor = ConsoleColor.Yellow;
		static int roundDigits = 5;
		static float epsilon = 0.1f;

		static void Main(string[] args)
		{
			Console.ForegroundColor = outputColor;

			Console.WriteLine("Input the first equation.(NO SPACES ALLOWED!,ONLY 2 TERMS.)");
			Console.WriteLine("The operators available are +,-,*,/,^ (power)");
			Console.Write("-");
			Console.ForegroundColor = inputColor;
			Eq1 = new Equation(Console.ReadLine());
			//Eq1 = new Equation("5x+3y=31");
			Console.ForegroundColor = outputColor;
			Console.WriteLine("Input the second equation.(NO SPACES ALLOWED!)");
			Console.Write("-");
			Console.ForegroundColor = inputColor;

			Eq2 = new Equation(Console.ReadLine());
			//Eq2 = new Equation("3x-y=13");
			Console.ForegroundColor = outputColor;

			Eq1.Print();
			Eq2.Print();

			Console.WriteLine("Do you want to see the steps to solve the equation?(YES/NO)");
		DetailException:
			Console.Write("-");

			Console.ForegroundColor = inputColor;
			string input = Console.ReadLine();
			Console.ForegroundColor = outputColor;
			if (input == "YES")
			{
				detailed = true;
			}
			else if (input == "NO")
			{
				detailed = false;
			}
			else
			{
				Console.WriteLine("INPUT WAS WRONG.(EXPECTED: YES/NO)");
				goto DetailException;
			}
			float min = 0;
			float max = 0;
			float increment = 0;
			Console.ForegroundColor = inputColor;
			if (Eq1.isLinear && Eq2.isLinear)
			{
				//If its linear, only need to detect 1 starting vertext and 1 end vertex. min must be a really small value and max must be a really high value and increment must go through the entire range at once.
				min = -10000;
				max = 10000;
				increment = 20000;
			}
			else
			{
				Console.ForegroundColor = outputColor;
				Console.WriteLine("input the range of checking.ie. -10,10,0.1 will check all numbers between -10 to 10 with a difference of 0.1(increments should be more than 0.01)");
				Console.Write("-");
				Console.ForegroundColor = inputColor;
				string[] range = Console.ReadLine().Split(',');

				min = float.Parse(range[0]);
				max = float.Parse(range[1]);
				increment = float.Parse(range[2]);
			}

			Console.ForegroundColor = ConsoleColor.Red;

			Console.WriteLine("Press enter to solve....");
		WrongKey:
			ConsoleKeyInfo key = Console.ReadKey();
			if (key.Key != ConsoleKey.Enter)
				goto WrongKey;
			Eq1.SetGraph(min, max, increment);
			Eq2.SetGraph(min, max, increment);
			Console.ForegroundColor = outputColor;
			if (detailed)
			{
				Console.WriteLine("First Equation's Vertices");
				Eq1.graph.Print();
				Console.WriteLine("Second Equation's Vertices");
				Eq2.graph.Print();
			}
			List<Vector> intersections = Eq1.graph.Intersects(Eq2.graph).Distinct().ToList();

			for (int i = 0; i < intersections.Count; i++)
			{
				Console.ForegroundColor = resultColor;
				Console.WriteLine("Line intersection found at: (" + intersections[i].x.ToString() + "," + intersections[i].y.ToString() + ")");
				Console.WriteLine("Rounded : (" + Math.Round(intersections[i].x, 3).ToString() + "," + Math.Round(intersections[i].y, 3).ToString() + ")");

			}
			Console.ForegroundColor = outputColor;
			if (intersections.Count == 0)
			{
				if (Eq1.isLinear && Eq2.isLinear) Console.WriteLine("Solution Does not exist");
				else Console.WriteLine("No solution found! Possibly due to set precision");
			}

			Console.WriteLine("Do you want to try another equation?(YES/NO)");
		LoopException:
			Console.ForegroundColor = inputColor;
			string answer = Console.ReadLine();
			Console.ForegroundColor = outputColor;
			if (answer == "YES")
			{
				Eq1.Clear();
				Eq2.Clear();
				Main(args);
			}
			else if (answer == "NO")
			{

			}
			else
			{
				Console.WriteLine("ANSWER NOT FOUND.RETYPE WITH 'YES' OR 'NO'");
				goto LoopException;
			}
			Console.WriteLine("Exception found press any key to turn off the program!!!");
			Console.ReadKey();
		}

		public class Equation
		{

			public Equation(string full)
			{
				whole = full;
				string[] sides = full.Split('=');
				leftHandSide = sides[0];
				rightHandSide = float.Parse(sides[1]);
				GetVariables();
			}

			public string whole;
			public string leftHandSide;
			public float rightHandSide;
			public bool isLinear = true;
			public List<Variable> vars = new List<Variable>();
			public Graph graph;
			void GetVariables()
			{
				string[] operands = new string[4];
				operands[0] = "+";
				operands[1] = "-";
				operands[2] = "*";
				operands[3] = "/";
				foreach (string operand in operands)
				{
					if (leftHandSide.Contains(operand))
					{
						string[] operandSplit = leftHandSide.Split(operand.ToCharArray());

						vars.Add(new Variable(operandSplit[0], "+"));
						if (vars[0].exponent != 1) isLinear = false;
						for (int i = 1; i < operandSplit.Length; i++)
						{
							if (operand == "*") vars[0].operand = "*";
							vars.Add(new Variable(operandSplit[i], operand));
							if (vars[i].exponent != 1 || vars[i].operand == "*")
								isLinear = false;
							
						}
					}
				}
				ShortenVars(vars);


			}
			public void SetGraph(float min, float max, float inc)
			{
				graph = new Graph(min, max, inc, this);
			}
			public void Print()
			{

				Console.WriteLine("Equation 1: The entire equation is: " + whole + " L.H.S is: " + leftHandSide + " R.H.S is: " + rightHandSide + " Variable Count is: " + vars.Count() + " The variables are: " + GetVarNames());
			}
			string GetVarNames()
			{
				string addedNames = "";
				for (int i = 0; i < vars.Count; i++)
				{
					addedNames += vars[i].operand + vars[i].multiplier + vars[i].name + "^" + vars[i].exponent + " ";
				}
				return addedNames;
			}
			public void Clear()
			{
				whole = null;
				leftHandSide = null;
				rightHandSide = 0;
				vars.Clear();

			}
			public bool CheckLinear()
			{
				foreach (Variable variable in vars)
				{
					if (variable.exponent > 1 || variable.exponent < 1)
						return false;
				}
				return true;
			}
			//WIP
			void ShortenVars(List<Variable> variables)
			{
				List<string> names = new List<string>();
				foreach (Variable variable in variables)
				{
					names.Add(variable.name);
				}
				for (int i = 0; i < names.Count; i++)
				{
					return;
				}
			}
		}
		public class Variable
		{
			public string name;
			public string operand;
			public float multiplier = 1;
			public float exponent = 1;
			public Variable(string sName, string sOperand)
			{
				#region Exponent and Coefficient
				string coefficient = "0";
				string power = "0";
				bool coeffLengthFound = false;
				bool expExists = sName.Contains("^");
				for (int i = 0; i < sName.Length; i++)
				{
					char c = sName.ElementAt(i);
					bool isDigit = char.IsDigit(c) || c == '.';
					if (isDigit && !coeffLengthFound)
						coefficient += c;
					else
						coeffLengthFound = true;
					if (isDigit && expExists && coeffLengthFound)
						power += c;
				}
				#endregion
				if (coefficient != "0") multiplier = (float)Math.Round(float.Parse(coefficient), roundDigits);

				if (power != "0") exponent = (float)Math.Round(float.Parse(power), roundDigits);
				name = sName.Replace(multiplier.ToString(), "");
				name = name.Replace("^" + exponent.ToString(), "");
				operand = sOperand;
				//Console.WriteLine($"multiplier {multiplier} exponent {exponent}");



			}
		}
		public class Graph
		{
			List<Vector> vertices = new List<Vector>();
			List<Line> edges = new List<Line>();
			public Graph(float min, float max, float increment, Equation e)
			{
				CreateGraph(min, max, increment, e);
			}
			public List<Vector> Intersects(Graph other)
			{
				List<Vector> Intersections = new List<Vector>();
				for (int i = 0; i < edges.Count; i++)
				{
					for (int j = 0; j < other.edges.Count; j++)
					{
						Vector Intersection = edges[i].Intersect(other.edges[j]);
						if (Intersection.exists) Intersections.Add(Intersection);
					}
				}
				return Intersections;
			}
			public void CreateGraph(float min, float max, float increment, Equation e)
			{
				for (float x = min; x <= max; x += increment)
				{
					x = (float)Math.Round(x, roundDigits);
					float tempX = (float)(Math.Pow(x, e.vars[0].exponent) * e.vars[0].multiplier);
					float y = 0;
					float RHS = e.rightHandSide;
					float yCoefficient = e.vars[1].multiplier;
					switch (e.vars[0].operand)
					{
						case "+":
							RHS -= tempX;
							break;
						case "-":
							RHS += tempX;
							break;
						case "*":
							RHS /= tempX;
							break;
						case "/":
							RHS *= tempX;
							break;
					}
					switch (e.vars[1].operand)
					{
						case "+":
							break;
						case "-":
							RHS *= -1;
							break;
						case "*":
							break;
						case "/":
							break;
					}
					yCoefficient = (float)Math.Pow(yCoefficient, 1 / e.vars[1].exponent);
					RHS = (float)Math.Pow(RHS, 1 / e.vars[1].exponent);
					y = RHS / yCoefficient;
					y = (float)Math.Round(y, roundDigits);
					vertices.Add(new Vector(x, y));
				}
				for (int i = 0; i < vertices.Count - 1; i++)
				{
					edges.Add(new Line(vertices[i], vertices[i + 1]));
				}
			}
			public void Print()
			{
				for (int i = 0; i < vertices.Count; i++)
				{
					Console.WriteLine("Vertex " + i + " : " + vertices[i].x + "," + vertices[i].y);
				}
			}

		}
		public class Line
		{
			Vector start;
			Vector end;

			public Line(Vector start, Vector end)
			{
				this.start = start;
				this.end = end;
			}

			public Vector Intersect(Line other)
			{
				float x1 = start.x;
				float y1 = start.y;
				float x2 = end.x;
				float y2 = end.y;
				float x3 = other.start.x;
				float y3 = other.start.y;
				float x4 = other.end.x;
				float y4 = other.end.y;

				float den = ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

				float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
				float u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / den;

				Vector P = new Vector(0, 0);

				if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
				{
					P.x = (x1 + t * (x2 - x1));
					P.y = (y1 + t * (y2 - y1));
				}
				else
				{
					P.exists = false;
				}
				return P;
			}
		}
		public class Vector
		{
			public float x;
			public float y;
			public bool exists = true;
			public Vector(float x, float y)
			{
				this.x = x;
				this.y = y;
			}
		}
	}
}
