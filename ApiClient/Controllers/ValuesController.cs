using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("pets")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public static List<Pet> pets = new List<Pet>();

        [HttpGet]
        public IEnumerable<Pet> getAllPets()
        {
            return pets;
        }

        [HttpPost]
        public Pet Save([FromBody] Pet pet)
        {
            pets.Add(pet);
            return pet;
        }
    }

    public class Pet
    {
        public string name { get; set; }

    }
}
