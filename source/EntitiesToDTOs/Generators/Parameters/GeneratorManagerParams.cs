using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Generators.Parameters
{
    /// <summary>
    /// Parameters for the GeneratorManager StartGeneration method.
    /// </summary>
    internal class GeneratorManagerParams
    {
        /// <summary>
        /// GenerateDTOs parameters.
        /// </summary>
        public GenerateDTOsParams DTOsParams { get; set; }

        /// <summary>
        /// GenerateAssemblers parameters.
        /// </summary>
        public GenerateAssemblersParams AssemblersParams { get; set; }

        /// <summary>
        /// Indicates if Assemblers must be generated.
        /// </summary>
        public bool GenerateAssemblers { get; set; }


        public GeneratorManagerParams() { }

        public GeneratorManagerParams(GenerateDTOsParams dtosParams, 
            GenerateAssemblersParams assemblersParams, bool generateAssemblers)
        {
            this.DTOsParams = dtosParams;
            this.AssemblersParams = assemblersParams;
            this.GenerateAssemblers = generateAssemblers;
        }
    }
}