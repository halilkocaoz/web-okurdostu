﻿using Microsoft.AspNetCore.Mvc;
using Okurdostu.Web.Models;

namespace Okurdostu.Web.Base
{
    [Route("api/[controller]")]
    public class ApiController : BaseController<ApiController>
    {
        public ActionResult Succes(JsonReturnModel jsonReturnModel)
        {
            jsonReturnModel.Code = 200;
            jsonReturnModel.Status = true;

            return Ok(jsonReturnModel);
        }
        public ActionResult Error(JsonReturnModel jsonReturnModel)
        {
            jsonReturnModel.Status = false;

            if (jsonReturnModel.Code == 401)
            {
                return Unauthorized();
            }
            else if (jsonReturnModel.Code == 403)
            {
                return Forbid();
            }
            else if (jsonReturnModel.Code == 404)
            {
                return NotFound(jsonReturnModel);
            }
            else if (jsonReturnModel.Code == 1001 || jsonReturnModel.Code == 200)
            {
                //10001: db'de değişiklik yapmaya çalışılırken hiç bir verinin değiştirilmediğini durumu: db.savechanges resultının 0 gelmesi.
                return Ok(jsonReturnModel);
            }

            jsonReturnModel.Code = 400;
            return BadRequest(jsonReturnModel);
        }
    }
}
