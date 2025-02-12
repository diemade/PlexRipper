﻿using System.ComponentModel.DataAnnotations.Schema;

namespace PlexRipper.Domain;

public class PlexAccountServer
{
    #region Relationships

    [Column(Order = 0)]
    public int PlexAccountId { get; set; }

    public virtual PlexAccount PlexAccount { get; set; }

    [Column(Order = 1)]
    public int PlexServerId { get; set; }

    public virtual PlexServer PlexServer { get; set; }

    #endregion

    #region Properties

    [Column(Order = 2)]
    public string AuthToken { get; set; }

    [Column(Order = 3)]
    public DateTime AuthTokenCreationDate { get; set; }

    #endregion
}