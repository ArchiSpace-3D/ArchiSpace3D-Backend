using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Hubs;
using ArchiSpace3D.Api.Service;
using ArchiSpace3D.Api.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<SalaColaborativaHub>("/hubs/sala");

app.Run();
