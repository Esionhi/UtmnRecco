using System;
using System.Collections.Generic;
using System.Linq;

namespace UtmnRecco.Models
{
	public enum Faculties : int
	{
		ИПиП = 165153,
		ИФК = 190438,
		//ИДПО = 879589,
		//ИДО = 2127387,
		ИМиКН = 2158741,
		ФТИ = 2158742,
		ИНБИО = 2158743,
		ИнЗем = 2158744,
		ИФИЖ = 2158745,
		ИИПН = 2158746,
		ИГиП = 2158747,
		ФЭИ = 2158748,
		ИнХим = 2175698,
		//СоцГум = 2235409,
		Target = -1
	}

	public static class FacultiesExtensions
	{
		public static IEnumerable<Faculties> EnumerateReal() => ((Faculties[])Enum.GetValues(typeof(Faculties))).Where(f => f != Faculties.Target);

		public static IEnumerable<Faculties> EnumerateWithTarget() => (Faculties[])Enum.GetValues(typeof(Faculties));
	}
}