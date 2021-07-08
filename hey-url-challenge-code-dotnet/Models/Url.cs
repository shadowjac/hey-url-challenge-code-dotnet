using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace hey_url_challenge_code_dotnet.Models
{
    public class Url
    {
        private readonly ILazyLoader lazyLoader;
        private ICollection<Historical> _historical;

        public Url()
        {

        }

        public Url(ILazyLoader lazyLoader)
        {
            Historical = new Collection<Historical>();
            this.lazyLoader = lazyLoader;
        }

        public Guid Id { get; set; }

        [Required]
        public string OriginalUrl { get; set; }

        [Required]
        [StringLength(5)]
        public string ShortUrl { get; set; }

        public int Count { get; set; }

        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Historical> Historical 
        {
            get => lazyLoader.Load(this, ref _historical);
            set => _historical = value;
        }
    }
}