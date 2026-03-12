using examenCrudMVC_NFramework1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace examenCrudMVC_NFramework1.ViewModel
{
    public class EmpleadoVM
    {
        public List<Empleado> ListaEmpleados {  get; set; } = new List<Empleado>();
        public Empleado EmpleadoModelReference { get; set; } = new Empleado();
        public List<Departamento> ListaDepartamentos { get; set; } = new List<Departamento>();
    }
}