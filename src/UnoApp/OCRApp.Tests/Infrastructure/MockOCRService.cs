using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using OCRApp.Models;
using OCRApp.Services;

namespace OCRApp.Tests.Infrastructure
{
    public class MockOCRService : IOCRService
    {
        public ExpectedLoginOutcome ExpectedLoginOutcome { get; set; }
        public bool ShouldFailSendImages { get; set; } = false;

        private int _currentGetResultIndex;

        private ImmutableArray<TryGetResultsForJobIdOutcome> _tryGetResultsForJobIdOutcomes;
        public ImmutableArray<TryGetResultsForJobIdOutcome> TryGetResultsForJobIdOutcomes
        {
            get => _tryGetResultsForJobIdOutcomes;
            set
            {
                _currentGetResultIndex = 0;
                _tryGetResultsForJobIdOutcomes = value;
            }
        }

        public string? LoggedInUsername { get; private set; }

        public Task<bool> LoginAsync(string username, string password)
        {
            if (ExpectedLoginOutcome == ExpectedLoginOutcome.Success)
            {
                LoggedInUsername = username;
                return Task.FromResult(true);
            }
            else if (ExpectedLoginOutcome == ExpectedLoginOutcome.Failure)
            {
                return Task.FromResult(false);
            }
            else if (ExpectedLoginOutcome == ExpectedLoginOutcome.Exception)
            {
                return Task.FromException<bool>(new Exception("Failing login."));
            }

            throw new Exception($"Unexpected ExpectedLoginOutcome value '{ExpectedLoginOutcome}'.");
        }

        public void Logout()
        {
            LoggedInUsername = null;
        }

        public Task<string> SendImages(IEnumerable<Uri> images)
        {
            if (ShouldFailSendImages)
            {
                return Task.FromException<string>(new Exception("Error sending images!"));
            }

            return Task.FromResult("JobId");
        }

        public Task<IEnumerable<string>?> TryGetResultsForJobIdAsync(string jobId)
        {
            var currentResult = TryGetResultsForJobIdOutcomes[_currentGetResultIndex++];
            if (currentResult == TryGetResultsForJobIdOutcome.InProgress)
            {
                return Task.FromResult<IEnumerable<string>?>(null);
            }
            else if (currentResult == TryGetResultsForJobIdOutcome.Done)
            {
                return Task.FromResult<IEnumerable<string>?>(new[] { "Output1", "Output2", "..." });
            }
            else
            {
                return Task.FromException<IEnumerable<string>?>(new Exception("Failing TryGetResultsForJobIdAsync"));
            }
        }

        Task<SignupResult> IOCRService.SignupAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}