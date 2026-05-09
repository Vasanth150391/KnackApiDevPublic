using Knack.API.Models;
using Knack.DBEntities;
using System.Runtime.CompilerServices;

namespace Knack.API.Mapping
{
    public static class TechnologyMapping
    {
        public static List<SolutionAreaPlayDTO> ToSolutionAreaPlayDTO(this List<SolutionArea> solutionAreas)
        {
            var solutionAreasplayDtos=new List<SolutionAreaPlayDTO>();
            foreach (var solutionArea in solutionAreas)
            {
                var solutionAreaPlayDTO = new SolutionAreaPlayDTO()
                {
                    SolutionAreaId = solutionArea.SolutionAreaId,
                    SolutionAreaName = solutionArea.SolutionAreaName,
                    ImageMain = solutionArea.Image_Main,
                    ImageMobile = solutionArea.Image_Mobile,
                    ImageThumb = solutionArea.Image_Thumb,
                    SolutionAreaDescription = solutionArea.SolAreaDescription,
                    SolutionAreaSlug=solutionArea.SolutionAreaSlug,                   
                  //  SolutionPlays = ToSolutionPlayDTO(solutionArea.SolutionPlay.ToList())
                };
                solutionAreasplayDtos.Add(solutionAreaPlayDTO);
            }
            return solutionAreasplayDtos;
        }

        public static SolutionAreaPlayDTO ToSolutionAreaPlaysDTO(this SolutionArea solutionArea)
        {           
                var solutionAreaPlayDTO = new SolutionAreaPlayDTO()
                {
                    SolutionAreaId = solutionArea.SolutionAreaId,
                    SolutionAreaName = solutionArea.SolutionAreaName,
                    ImageMain = solutionArea.Image_Main,
                    ImageMobile = solutionArea.Image_Mobile,
                    ImageThumb = solutionArea.Image_Thumb,
                    SolutionAreaDescription = solutionArea.SolAreaDescription,
                    SolutionAreaSlug = solutionArea.SolutionAreaSlug,
                   // SolutionPlays = ToSolutionPlayDTO(solutionArea.SolutionPlay.ToList())
                };
              
            return solutionAreaPlayDTO;
        }
        public static List<SolutionAreaDTO> ToSolutionAreaDTO(this List<SolutionArea> solutionAreas)
        {
            var solutionAreasDto = new List<SolutionAreaDTO>();
            foreach (var solutionArea in solutionAreas) {
                var solutionAreaDto = new SolutionAreaDTO
                {
                    SolutionAreaId = solutionArea.SolutionAreaId,
                    SolutionAreaName = solutionArea.SolutionAreaName,
                    ImageMain = solutionArea.Image_Main,
                    ImageMobile = solutionArea.Image_Mobile,
                    ImageThumb = solutionArea.Image_Thumb,
                    SolAreaDescription = solutionArea.SolAreaDescription,
                    SolutionAreaSlug = solutionArea.SolutionAreaSlug                    
                };
                solutionAreasDto.Add(solutionAreaDto);
           }
            return solutionAreasDto;
        }

        public static List<SolutionPlayDTO> ToSolutionPlayDTO(this List<SolutionPlay> solutionPlays)
        {            
            var solutionPlaysDto = new List<SolutionPlayDTO>();
            foreach (var solutionPlay in solutionPlays)
            {
                var solutionPlayDto = new SolutionPlayDTO()
                {
                    SolutionPlayLabel = solutionPlay.SolutionPlayLabel,
                    ImageMain = solutionPlay.Image_Main,
                    ImageMobile = solutionPlay.Image_Mobile,
                    ImageThumb = solutionPlay.Image_Thumb,
                    IsPublished = solutionPlay.IsPublished,
                    RowChangedBy = solutionPlay.RowChangedBy,
                    RowChangedDate = solutionPlay.RowChangedDate,
                    SolutionAreaId = solutionPlay.SolutionAreaId,
                    SolutionPlayDesc = solutionPlay.SolutionPlayDesc,
                    SolutionPlayId = solutionPlay.SolutionPlayId,
                    SolutionPlayName = solutionPlay.SolutionPlayName,
                    SolutionStatusId = solutionPlay.SolutionStatusId   ,
                   // SolutionStatus=solutionPlay.SolutionStatus.SolutionStatus,
                    SolutionPlayThemeSlug = solutionPlay.SolutionPlayThemeSlug
                };
                solutionPlaysDto.Add(solutionPlayDto);
            }

            return solutionPlaysDto;
        }
    }
}
