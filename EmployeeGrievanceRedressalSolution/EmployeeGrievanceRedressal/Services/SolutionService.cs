﻿using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Interfaces.ServiceInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Models.DTOs.Grievance;
using EmployeeGrievanceRedressal.Models.DTOs.Solution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressal.Services
{
    public class SolutionService : ISolutionService
    {
        private readonly IGrievanceRepository _grievanceRepository;
        private readonly ISolutionRepository _solutionRepository;
        private readonly IGrievanceHistoryService _grievanceHistoryService;

        public SolutionService(IGrievanceRepository grievanceRepository, ISolutionRepository solutionRepository, IGrievanceHistoryService grievanceHistoryService)
        {
            _grievanceRepository = grievanceRepository;
            _solutionRepository = solutionRepository;
            _grievanceHistoryService = grievanceHistoryService;
        }

        public async Task<SolutionDTO> GetSolutionByIdAsync(int id)
        {
            try
            {
                var solution =await _solutionRepository.GetSolutionByIdwithSolver(id);
                if(solution == null)
                {
                    throw new EntityNotFoundException("No Solution with id");
                }
                return new SolutionDTO
                {
                    SolutionId = solution.SolutionId,
                    SolutionTitle = solution.SolutionTitle,
                    GrievanceId = solution.GrievanceId,
                    SolverId = solution.SolverId == null ? null : (int)solution.SolverId,
                    SolverName = solution.Solver?.Name,
                    Description = solution.Description,
                    DateProvided = solution.DateProvided,
                    DocumentUrls = solution.DocumentUrls.Select(d => d.Url).ToList()
                };
            }
            catch(Exception ex)
            {
                throw new RepositoryException("Error in fetching the solution", ex);
            }
        }

        public async Task<SolutionDTO> ProvideSolutionAsync(ProvideSolutionDTO dto)
        {
            try {
            // Validate if the grievance exists
            var grievance = await _grievanceRepository.GetByIdAsync(dto.GrievanceId);
            if (grievance == null)
            {
                throw new EntityNotFoundException($"Grievance with ID {dto.GrievanceId} not found.");
            }

            if(grievance.Status != GrievanceStatus.InProgress)
            {
                throw new Exception("Can Provide solution only to open grievances !!");
            }

                // Map DTO to Solution entity
                // Map DTO to Solution entity
                var solution = new Solution
                {
                    GrievanceId = dto.GrievanceId,
                    SolverId = dto.SolverId,
                    SolutionTitle = dto.SolutionTitle,
                    Description = dto.Description,
                    DateProvided = DateTime.UtcNow
                };

                // Save the solution without DocumentUrls
                await _solutionRepository.Add(solution);

                // Retrieve the SolutionId
                var solutionId = solution.SolutionId;

                // Update DocumentUrls with the SolutionId
                if (dto.DocumentUrls != null && dto.DocumentUrls.Any())
                {
                    var documentUrls = dto.DocumentUrls.Select(url => new DocumentUrl
                    {
                        Url = url,
                        GrievanceId = dto.GrievanceId,
                        SolutionId = solutionId // Add the SolutionId here
                    }).ToList();

                    solution.DocumentUrls = documentUrls;
                    _solutionRepository.Update(solution);
                }

                var solutiondto =  new SolutionDTO
                {
                    SolutionId = solution.SolutionId,
                    GrievanceId = solution.GrievanceId,
                    SolverId = solution.SolverId == null ? null : (int)solution.SolverId,
                    SolverName = solution.Solver?.Name,
                    SolutionTitle = solution.SolutionTitle,
                    Description = solution.Description,
                    DateProvided = solution.DateProvided,
                    DocumentUrls = solution?.DocumentUrls?.Select(d => d.Url).ToList()
                };
                var history = new GrievanceHistory
                {
                    GrievanceId = solution.GrievanceId,
                    HistoryType = "Solution",
                    RelatedEntityId = solution.SolutionId,
                    DateChanged = DateTime.UtcNow,
                    StatusChange = $"{solution.SolutionTitle}",
                };
                var historyDto = await _grievanceHistoryService.RecordHistoryAsync(history);
                return solutiondto;

            }
            catch (Exception ex)
            {
                throw new ServiceException("Error providing solution", ex);
            }
        }
        public async Task<IEnumerable<SolutionDTO>> GetSolutionsByGrievanceIdAsync(int grievanceId)
        {
            var grievance = await _grievanceRepository.GetByIdAsync(grievanceId);
            if (grievance == null)
            {
                throw new EntityNotFoundException($"Grievance with ID {grievanceId} not found.");
            }

            var solutions = await _solutionRepository.GetAllSolutionswithSolver(); // Adjust if needed to filter by grievanceId

            return solutions
                .Where(s => s.GrievanceId == grievanceId)
                .Select(s => new SolutionDTO
                {
                    SolutionId = s.SolutionId,
                    GrievanceId = s.GrievanceId,
                    SolverId = s.SolverId == null ? null : (int)s.SolverId,
                    SolverName = s.Solver?.Name,
                    SolutionTitle = s.SolutionTitle,
                    Description = s.Description,
                    DateProvided = s.DateProvided,
                    DocumentUrls = s.DocumentUrls?.Select(d => d.Url).ToList()
                });
        }
    }
}
