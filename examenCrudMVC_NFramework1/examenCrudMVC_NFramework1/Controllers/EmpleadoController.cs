using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using examenCrudMVC_NFramework1.Models;
using examenCrudMVC_NFramework1.ViewModel;
using System.Data.SqlClient;

namespace examenCrudMVC_NFramework1.Controllers
{
    public class EmpleadoController : Controller
    {
        private examenCrudMVC_NFramework1Entities _db = new examenCrudMVC_NFramework1Entities();

        public async Task<ActionResult> Index(int? idDep)
        {
            var vm = new EmpleadoVM();
            ViewBag.IdSeleccionado = idDep;
            try
            {
                vm.ListaDepartamentos = await _db.Departamentos.ToListAsync();
                if (idDep>0)
                {
                    vm.ListaEmpleados = await _db.Empleados.
                        SqlQuery("exec sp_ListarEmpleadoPorIdDep @idDepartamento", new SqlParameter("@idDepartamento", idDep)).
                        ToListAsync();

                    foreach(var emp in vm.ListaEmpleados)
                    {
                        emp.IdDepartamentoNavigation = vm.ListaDepartamentos.
                            FirstOrDefault(d => d.idDepartamento == emp.idDepartamento);
                    }
                }
                else
                {
                    vm.ListaEmpleados = await _db.Empleados.
                        Include(tD => tD.IdDepartamentoNavigation).
                        OrderByDescending(a => a.activo).
                        ToListAsync();
                }
                return View(vm);

            }catch (Exception ex)
            {
                vm.ListaEmpleados = new List<Empleado>();
                vm.ListaDepartamentos = new List<Departamento>();
                Console.WriteLine(ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Guardar(EmpleadoVM vm)
        {
            try
            {
                vm.EmpleadoModelReference.activo = true;

                _db.Empleados.Add(vm.EmpleadoModelReference);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int? idEmp)
        {
            var vm = new EmpleadoVM();
            try
            {
                vm.ListaDepartamentos = await _db.Departamentos.ToListAsync();
                if (idEmp == null) return RedirectToAction("Index");
                var empleado = await _db.Empleados.FindAsync(idEmp);
                if(empleado == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    vm.EmpleadoModelReference = empleado;
                    return View(vm);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(EmpleadoVM vm)
        {
            try
            {
                _db.Entry(vm.EmpleadoModelReference).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(int? idEmp, int? idDep)
        {

            try
            {
                var empleado = await _db.Empleados.FindAsync(idEmp);
                if(empleado == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    _db.Empleados.Remove(empleado);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
            return RedirectToAction("Index", new {idDep = idDep});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Estado(int? idEmp, int? idDep)
        {
            try
            {
                await _db.Database.ExecuteSqlCommandAsync("exec sp_EstadoEmpleado {0}", idEmp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            return RedirectToAction("Index", new { idDep = idDep });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
