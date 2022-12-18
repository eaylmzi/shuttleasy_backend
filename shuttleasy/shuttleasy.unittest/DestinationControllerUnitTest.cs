using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Moq;
using shuttleasy.Controllers;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Resource.String;
using shuttleasy.LOGIC.Logics.Destinations;
using shuttleasy.Models.dto.Credentials.dto;
using shuttleasy.Models.dto.Destinations.dto;
using shuttleasy.Services;
using System.Net;
using System.Web;

namespace shuttleasy.unittest
{
    public class DestinationControllerUnitTest
    {


        [Fact]
        public void Test1()
        {
            var mockUserService = getDefaultUserService();
            var mockMapperService = new Mock<IMapper>();
            var mockDestinationService = new Mock<IDestinationLogic>();
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request.Headers[HeaderNames.Authorization]).Returns("eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsImlkIjoiMzgiLCJyb2xlIjoiYWRtaW4iLCJleHAiOjE2OTQ5NTUxMjJ9.G7bqmXb8c3kPe3WFPUjutcxHVOFLXsqjSAmgHKhuQDubxn5dNK-02yQH0lmE2F6dXyf1Qfjc8GlcI3_wAfFinw");
           
            UserVerifyingDto userVerifyingDto = new UserVerifyingDto()
            {
                Id = 38,
                Role = Roles.Admin,
                Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsImlkIjo" +
                "iMzgiLCJyb2xlIjoiYWRtaW4iLCJleHAiOjE2OTQ5NTUxMjJ9.G7bqmXb8c3kPe3WFPUjutcxHVOFLXsqjSAmgHKhuQDubxn5dNK-02yQH0lmE2F6dXyf1Qfjc8GlcI3_wAfFinw"
            };


            DestinationDto destinationDto = new DestinationDto()
            {
                CityNumber = 35,
                BeginningDestination = "bölg",
                LastDestination = "konak"
            };

            Destination destination = new Destination()
            {

                CityNumber = 35,
                BeginningDestination = "bölg",
                LastDestination = "konak"
            };

            mockUserService.Setup(x => x.VerifyUser(new UserVerifyingDto())).Returns(true);
            mockMapperService.Setup(x => x.Map<Destination>(destinationDto)).Returns(destination);
            mockDestinationService.Setup(x => x.Add(destination)).Returns(true);

            var destinationController = new DestinationController(mockUserService.Object, mockDestinationService.Object,
                mockMapperService.Object);

            HttpContext.Current = mockHttpContext.Object;
            int userId = destinationController.GetUserIdFromRequestToken();
            var result = destinationController.AddDestination(destinationDto);
            Assert.True((result.Value));
        }




/*
        [Fact]
        public void TestHeaderValuesWithMocks()
        {
            // Mock HttpClient nesnesini oluþturun
            var mockClient = new Mock<HttpClient>();

            // Mock HttpResponseMessage nesnesini oluþturun ve beklenen Header deðerlerini ekleyin
            var mockResponse = new Mock<HttpResponseMessage>();
            mockResponse.Object.Headers.Add("admin", "bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsImlkIjoiMzgiLCJyb2xlIjoiYWRtaW4iLCJleHAiOjE2OTQ5NTUxMjJ9.G7bqmXb8c3kPe3WFPUjutcxHVOFLXsqjSAmgHKhuQDubxn5dNK-02yQH0lmE2F6dXyf1Qfjc8GlcI3_wAfFinw");


            // HttpClient nesnesinin SendAsync metodunu mock'layýn ve mockResponse nesnesini döndürün
            mockClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).Returns(Task.FromResult(mockResponse.Object));

            // API adresini ve gerekli parametreleri tanýmlayýn
            var client = mockClient.Object;
            var request = new HttpRequestMessage(HttpMethod.Post, "https://shuttleasydatabase.azurewebsites.net/api/Destination/AddDestination");

            // Header'dan gelen deðerleri ekleyin
            request.Headers.Add("admin", "bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsImlkIjoiMzgiLCJyb2xlIjoiYWRtaW4iLCJleHAiOjE2OTQ5NTUxMjJ9.G7bqmXb8c3kPe3WFPUjutcxHVOFLXsqjSAmgHKhuQDubxn5dNK-02yQH0lmE2F6dXyf1Qfjc8GlcI3_wAfFinw");


            // Ýstek gönderin ve yanýtý alýn
            var response = client.SendAsync(request).Result;

            // Yanýt içinde beklenen Header deðerlerini kontrol edin
            Assert.Equal("bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsImlkIjoiMzgiLCJyb2xlIjoiYWRtaW4iLCJleHAiOjE2OTQ5NTUxMjJ9.G7bqmXb8c3kPe3WFPUjutcxHVOFLXsqjSAmgHKhuQDubxn5dNK-02yQH0lmE2F6dXyf1Qfjc8GlcI3_wAfFinw", response.Headers.GetValues("admin").FirstOrDefault());

        }
*/

        







    




        private Mock<IUserService> getDefaultUserService()
        {
            var mockUserService = new Mock<IUserService>();
            return mockUserService;
        }
    }
}