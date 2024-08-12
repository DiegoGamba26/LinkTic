using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEBAPI.Models; // Reemplaza con el nombre correcto del espacio de nombres
using Microsoft.EntityFrameworkCore;
using WEBAPI.Data;
using WEBAPI.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace WEBAPI.Controllers // Reemplaza con el nombre correcto del espacio de nombres
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Asegura que el usuario esté autenticado

    public class ReservationsController : ControllerBase
    {
        private readonly prueba_tecnicaContext _dbContext;

        public ReservationsController(prueba_tecnicaContext context)
        {
            _dbContext = context;
        }

        [HttpPost]
        [Route("createReservation")]

        public IActionResult CreateReservation([FromBody] ReservationDTO reservationDto)
        {
            if (reservationDto == null)
            {
                return BadRequest("Reservation data is required.");
            }

            // Validar si el cliente y el servicio existen
            var customerExists = _dbContext.Customers.Any(c => c.CustomerId == reservationDto.CustomerId);
            var serviceExists = _dbContext.Services.Any(s => s.ServiceId == reservationDto.ServiceId);

            if (!customerExists || !serviceExists)
            {
                return BadRequest(new
                {
                    message = "Invalid Customer or Service.",
                    errors = new
                    {
                        CustomerExists = customerExists,
                        ServiceExists = serviceExists
                    }
                });
            }

            // Obtener el UserId del token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = int.Parse(userIdClaim.Value);

            var reservation = new Reservation
            {
                CustomerId = reservationDto.CustomerId,
                ServiceId = reservationDto.ServiceId,
                ReservationDate = reservationDto.ReservationDate,
                Status = reservationDto.Status,
                CreatedAt = DateTime.Now
            };

            try
            {
                _dbContext.Reservations.Add(reservation);
                _dbContext.SaveChanges();

                // Asociar la reserva con el usuario en la tabla intermedia
                var userReservation = new UserReservation
                {
                    UserId = userId,
                    ReservationID = reservation.ReservationId
                };
                _dbContext.UserReservations.Add(userReservation);
                _dbContext.SaveChanges();

                return Ok(new { message = "Reservation created successfully.", reservationId = reservation.ReservationId });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating reservation", error = ex.Message });
            }
        }

        // Endpoint para obtener reservas con filtrado por fecha, servicio o cliente
        [HttpGet]
        [Route("getReservations")]
        public IActionResult GetReservations(DateTime? startDate, DateTime? endDate, int? serviceId, int? customerId)
        {
            try
            {
                var query = _dbContext.Reservations
                    .Include(r => r.Customer)
                    .Include(r => r.Service)
                    .AsQueryable();

                // Aplicar filtros si se proporcionan
                if (startDate.HasValue)
                {
                    query = query.Where(r => r.ReservationDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(r => r.ReservationDate <= endDate.Value);
                }

                if (serviceId.HasValue)
                {
                    query = query.Where(r => r.ServiceId == serviceId.Value);
                }

                if (customerId.HasValue)
                {
                    query = query.Where(r => r.CustomerId == customerId.Value);
                }

                var reservations = query.ToList();

                return Ok(new { message = "Reservations retrieved successfully", data = reservations });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving reservations", error = ex.Message });
            }
        }

        // Endpoint para actualizar una reserva
        [HttpPut]
        [Route("updateReservation/{id}")]
        public IActionResult UpdateReservation(int id, [FromBody] ReservationDTO reservationDto)
        {
            if (reservationDto == null)
            {
                return BadRequest("Reservation data is required.");
            }

            try
            {
                var existingReservation = _dbContext.Reservations.Find(id);
                if (existingReservation == null)
                {
                    return NotFound("Reservation not found.");
                }

                // Validar si el cliente y el servicio existen
                var customerExists = _dbContext.Customers.Any(c => c.CustomerId == reservationDto.CustomerId);
                var serviceExists = _dbContext.Services.Any(s => s.ServiceId == reservationDto.ServiceId);

                if (!customerExists || !serviceExists)
                {
                    return BadRequest("Invalid Customer or Service.");
                }

                // Mapear los valores del DTO al modelo
                existingReservation.CustomerId = reservationDto.CustomerId;
                existingReservation.ServiceId = reservationDto.ServiceId;
                existingReservation.ReservationDate = reservationDto.ReservationDate;
                existingReservation.Status = reservationDto.Status;
                existingReservation.UpdatedAt = DateTime.Now;

                _dbContext.Reservations.Update(existingReservation);
                _dbContext.SaveChanges();

                return Ok(new { message = "Reservation updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating reservation", error = ex.Message });
            }
        }

        // Endpoint para eliminar una reserva
        [HttpDelete]
        [Route("deleteReservation/{id}")]
        public IActionResult DeleteReservation(int id)
        {
            try
            {
                var reservation = _dbContext.Reservations.Find(id);
                if (reservation == null)
                {
                    return NotFound("Reservation not found.");
                }

                _dbContext.Reservations.Remove(reservation);
                _dbContext.SaveChanges();
                return Ok(new { message = "Reservation deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting reservation", error = ex.Message });
            }
        }

        // Endpoint para crear un cliente
        [HttpPost]
        [Route("createCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customerDTO)
        {
            if (customerDTO == null)
            {
                return BadRequest("Customer data is required.");
            }

            try
            {
                var customer = new Customer
                {
                    FirstName = customerDTO.FirstName,
                    LastName = customerDTO.LastName,
                    Email = customerDTO.Email,
                    Phone = customerDTO.Phone,
                    CreatedAt = DateTime.Now
                };
                _dbContext.Customers.Add(customer);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, new { message = "Customer created successfully", data = customer });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating customer", error = e.Message });
            }
        }

        // Endpoint para crear un servicio
        [HttpPost]
        [Route("createService")]
        public async Task<IActionResult> CreateService([FromBody] ServiceDTO serviceDTO)
        {
            if (serviceDTO == null)
            {
                return BadRequest("Service data is required.");
            }

            try
            {
                var service = new Service
                {
                    ServiceName = serviceDTO.ServiceName,
                    Description = serviceDTO.Description,
                    Price = serviceDTO.Price,
                    Duration = serviceDTO.Duration,
                    CreatedAt = DateTime.Now
                };
                _dbContext.Services.Add(service);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, new { message = "Service created successfully", data = service });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating service", error = e.Message });
            }
        }

        // Endpoint para crear un usuario
        [HttpPost]
        [Route("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            if (userDTO == null)
            {
                return BadRequest("User data is required.");
            }

            try
            {
                var user = new Users
                {
                    UserName = userDTO.UserName,
                    UserEmail = userDTO.UserEmail,
                    UserPassword = userDTO.UserPassword, // Consider hashing passwords before saving them
                    CreatedAt = DateTime.Now
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new { message = "User created successfully", data = user });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating user", error = e.Message });
            }
        }
        [HttpGet]
        [Route("currentUser")]
        public IActionResult GetCurrentUser()
        {
            var user = User.Identity;
            if (user.IsAuthenticated)
            {
                var claims = User.Claims.Select(c => new { c.Type, c.Value });
                return Ok(new { User.Identity.Name, Claims = claims });
            }
            return Unauthorized("User is not authenticated");
        }

        [HttpGet]
        [Route("getCustomers")]
        public IActionResult GetCustomers()
        {
            try
            {
                var query = _dbContext.Customers.AsQueryable();

                var customers = query.ToList();

                return Ok(new { message = "Reservations retrieved successfully", data = customers });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving reservations", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("getServices")]
        public IActionResult GetServices()
        {
            try
            {
                var query = _dbContext.Services.AsQueryable();

                var services = query.ToList();

                return Ok(new { message = "Reservations retrieved successfully", data = services });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving reservations", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("userReservations")]
        public IActionResult GetUserReservations()
        {
            // Recuperar el UserId desde el token JWT
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            // Consultar las reservas asociadas al usuario
            var reservations = _dbContext.UserReservations
                .Where(ur => ur.UserId == userId)
                .Select(ur => new
                {
                    ur.Reservation.ReservationId,
                    ur.Reservation.Customer.FirstName,
                    ur.Reservation.Customer.LastName,
                    ur.Reservation.Service.ServiceName,
                    ur.Reservation.ReservationDate,
                    ur.Reservation.Status
                })
                .ToList();

            if (reservations == null || !reservations.Any())
            {
                return NotFound("No reservations found for this user.");
            }

            return Ok(reservations);
        }

    }
}
