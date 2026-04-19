export interface Comment {
  id: number;
  text: string;
  userName: string;
  createdAt: string;
  parentId?: number;
  textFileId: string;
  textFileName?: string;
  imagePreviewUrl?: string;
  imageOriginalUrl?: string;
  replyCount: number;
}