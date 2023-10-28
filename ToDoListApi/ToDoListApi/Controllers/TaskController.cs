using Azure;
using Microsoft.AspNetCore.Mvc;
using ToDoListApi.Dtos;
using ToDoListApi.Services;

namespace ToDoListApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskServices _taskServices;
        public TaskController(ITaskServices taskServices)
        {
            _taskServices = taskServices;

        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] TaskFilterDto input)
        {
            try
            {
                var taskLst =  _taskServices.GetAll(input);

                return Ok(new
                {
                    isSuccess = true,
                    count = taskLst.Count(),
                    items = taskLst,
                    message = "Operation réussi"
                });
            }catch (Exception ex)
            {
                return Ok(new List<Task>());

            }
        }


        [HttpPost("createTask")]
        public async Task<IActionResult> Create([FromBody] TaskDto input)
        {

            if (input == null)
            {
                return BadRequest(); 
            }

            try
            {
                var taskID = await _taskServices.CreateTask(input);

                return Ok(
                    new
                    {
                        isSuccess = true,
                        id = taskID,
                        message = "Operation réussi"
                    }
                    );


            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPut("updateTask")]
        public async Task<IActionResult> Update([FromBody] TaskDto input)
        {
            try
            {
                var taskID = await _taskServices.UpdateTask(input);

                return Ok(
                    new
                    {
                        isSuccess = true,
                        id = taskID,
                        message = "Operation réussi"

                    });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }
        
        [HttpPost("deleteTask")]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            try
            {
                var taskID = await _taskServices.DeleteTask(id);

                return Ok(
                    new
                    {
                        isSuccess = true,
                        id = taskID,
                        message = "Operation réussi"

                    });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }
    }
}
